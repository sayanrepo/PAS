using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using BaseSite.Api.Queries;
using BaseSite.Models.DBModel;
using BaseSite.Web.Models;
using BaseSite.Web.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

internal static class OrderPrintChecks
{
    public static async Task RunAsync(Action<bool, string> check, bool preview = false)
    {
        var source = Sample();
        var invoice = WebOrder(OrderPrintDataMapper.ForPrint(source, "invoice"));
        OrderPrintPresentation.Prepare(invoice, "invoice");
        check(invoice.Order_Cabin[0].Cost == 600000 && source.Order_Cabin.Single().Cost == 1000000,
            "Invoice splits monitor, attachment and addition costs without mutating the stored order");
        var bill = WebOrder(OrderPrintDataMapper.ForPrint(source, "bill"));
        OrderPrintPresentation.Prepare(bill, "bill");
        check(bill.Order_Cabin[0].Cost == 900000, "Bill splits only printable attachment IDs 5 and 65");
        var specification = OrderPrintDataMapper.ForPrint(source, "specification");
        check(specification.Cost == 0 && specification.Order_Cabin[0].Cost == 0
            && specification.Order_Cabin[0].CostMonitor == 0
            && specification.Order_Cabin[0].Order_Panel_Attachment[0].Cost == 0
            && specification.Order_Cabin[0].Order_Panel_Addition[0].Cost == 0
            && specification.Account_Users.EconomicalNumber == "",
            "Specification permissions never expose financial or billing identity data");
        var controller = new BaseSite.Api.Controllers.OrderPrintController
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, "Plan_Print")], "test"))
            } }
        };
        check(controller.Print(1, "bill") is ForbidResult && controller.Print(1, "invoice") is ForbidResult,
            "Plan_Print alone cannot request either financial print kind");
        check(controller.Print(1, "unknown") is NotFoundResult && controller.Print(0, "specification") is NotFoundResult,
            "Invalid kind and document ID are rejected before any database access");

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(BaseSite.Web.Pages.Print.Orders.InvoiceModel).Assembly.GetName().Name,
            EnvironmentName = "Development", ContentRootPath = WebRoot()
        });
        builder.Logging.ClearProviders();
        builder.Services.AddRazorPages();
        builder.Services.AddAntiforgery(o => o.HeaderName = "X-CSRF-TOKEN");
        builder.Services.AddSingleton<IDataProtectionProvider>(new EphemeralDataProtectionProvider());
        builder.Services.AddSingleton<PrintSessionCookie>();
        builder.Services.AddTransient(_ => new OrderPrintClient(new HttpClient(new FakePrintApi())
            { BaseAddress = new Uri("http://print-test.invalid/") }));
        builder.Services.AddTransient(_ => new SalePrintClient(new HttpClient(new FakePrintApi())
            { BaseAddress = new Uri("http://print-test.invalid/") }));
        builder.Services.AddTransient(_ => new ServicePrintClient(new HttpClient(new FakePrintApi())
            { BaseAddress = new Uri("http://print-test.invalid/") }));
        builder.Services.AddTransient(_ => new PaymentPrintClient(new HttpClient(new FakePrintApi())
            { BaseAddress = new Uri("http://print-test.invalid/") }));
        builder.Services.AddTransient(_ => new DeliveryPrintClient(new HttpClient(new FakePrintApi())
            { BaseAddress = new Uri("http://print-test.invalid/") }));
        await using var app = builder.Build();
        app.UseStaticFiles();
        app.UseAntiforgery();
        app.MapRazorPages();
        if (preview)
            app.MapGet("/preview", (HttpContext context, PrintSessionCookie cookies) =>
            {
                var login = new BaseSite.Web.Models.LoginResponse
                    { AccessToken = "print-test-token", ExpiresAt = DateTimeOffset.UtcNow.AddHours(1) };
                cookies.Write(context, cookies.Create(login), login.ExpiresAt);
                return Results.Redirect("/print/orders/1/bill");
            });
        app.Urls.Add(preview ? "http://localhost:5088" : "http://127.0.0.1:0");
        await app.StartAsync();
        if (preview)
        {
            Console.WriteLine("Isolated print preview: http://localhost:5088/preview");
            await app.WaitForShutdownAsync();
            return;
        }
        try
        {
            using var handler = new HttpClientHandler { CookieContainer = new CookieContainer() };
            using var browser = new HttpClient(handler) { BaseAddress = new Uri(app.Urls.Single()) };
            var cookies = app.Services.GetRequiredService<PrintSessionCookie>();
            var ticket = cookies.Create(new() { AccessToken = "print-test-token", ExpiresAt = DateTimeOffset.UtcNow.AddHours(1) });
            check(cookies.Read(ticket + "bad") is null && cookies.Read(cookies.Create(new()
                { AccessToken = "expired", ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(-1) })) is null,
                "Tampered and expired print credentials are rejected");
            check((await browser.GetAsync("/print/orders/1/invoice")).StatusCode == HttpStatusCode.Unauthorized,
                "Direct print requests require authentication without a Blazor circuit");
            check((await browser.PostAsync("/print/session", Form(ticket))).StatusCode == HttpStatusCode.BadRequest,
                "The session bridge rejects requests without antiforgery proof");
            var csrf = await browser.GetFromJsonAsync<JsonElement>("/print/session");
            browser.DefaultRequestHeaders.Add("X-CSRF-TOKEN", csrf.GetProperty("requestToken").GetString());
            check((await browser.PostAsync("/print/session", Form("forged"))).StatusCode == HttpStatusCode.Unauthorized,
                "A forged session ticket cannot establish print authentication");
            var login = await browser.PostAsync("/print/session", Form(ticket));
            check(login.StatusCode == HttpStatusCode.NoContent
                && login.Headers.GetValues("Set-Cookie").Any(x => x.Contains("httponly", StringComparison.OrdinalIgnoreCase)
                    && x.Contains("path=/print", StringComparison.OrdinalIgnoreCase)),
                "The session bridge installs an HttpOnly cookie scoped to print pages");
            foreach (var kind in new[] { "specification", "invoice", "bill" })
            {
                var result = await browser.GetAsync($"/print/orders/1/{kind}");
                var html = await result.Content.ReadAsStringAsync();
                check(result.IsSuccessStatusCode && result.Content.Headers.ContentType?.MediaType == "text/html"
                    && html.Contains($"data-print-kind=\"{kind}\"") && html.Contains("<!DOCTYPE html>") && html.Contains("print-document.js")
                    && !html.Contains("blazor.web.js") && !html.Contains("<script>injected</script>")
                    && result.Headers.CacheControl?.NoStore == true,
                    $"{kind} renders a complete encoded HTML document with print controls and no circuit");
                var again = await browser.GetStringAsync($"/print/orders/1/{kind}");
                check(html == again, $"Refreshing {kind} fetches a fresh snapshot without subtracting costs twice");
            }
            foreach (var status in new[] { 401, 403, 404, 500 })
                check((int)(await browser.GetAsync($"/print/orders/{status}/invoice")).StatusCode == (status == 500 ? 502 : status),
                    $"API status {status} is handled by the independent print page");
            foreach (var kind in new[] { "bill", "invoice" })
            {
                var result = await browser.GetAsync($"/print/sales/1/{kind}");
                var html = await result.Content.ReadAsStringAsync();
                check(result.IsSuccessStatusCode && result.Content.Headers.ContentType?.MediaType == "text/html"
                    && html.Contains("<!DOCTYPE html>") && html.Contains("/Contents/js/printThis.js")
                    && (kind != "bill" || html.Contains("چاپ / ذخیره PDF")),
                    $"Goods-sale {kind} renders as an authenticated standalone legacy Razor page");
            }
            var serviceResult = await browser.GetAsync("/print/services/1/invoice");
            var serviceHtml = await serviceResult.Content.ReadAsStringAsync();
            check(serviceResult.IsSuccessStatusCode && serviceResult.Content.Headers.ContentType?.MediaType == "text/html"
                && serviceHtml.Contains("<!DOCTYPE html>") && serviceHtml.Contains("/Contents/js/printThis.js")
                && serviceHtml.Contains("چاپ / ذخیره PDF"),
                "Service invoice renders as an authenticated standalone legacy Razor page");
            foreach (var kind in new[] { "accounting", "customer" })
            {
                var result = await browser.GetAsync($"/print/payments/1/{kind}");
                var html = await result.Content.ReadAsStringAsync();
                check(result.IsSuccessStatusCode && result.Content.Headers.ContentType?.MediaType == "text/html"
                    && html.Contains("<!DOCTYPE html>") && html.Contains("/Contents/js/printThis.js")
                    && html.Contains("چاپ / ذخیره PDF")
                    && html.Contains(kind == "accounting" ? "نسخه حسابداری" : "نسخه مشتری"),
                    $"Received-document {kind} receipt renders as an authenticated standalone legacy Razor page");
            }
            foreach (var kind in new[] { "delivery", "pack", "panel" })
            {
                var result = await browser.GetAsync($"/print/deliveries/1/{kind}");
                var html = await result.Content.ReadAsStringAsync();
                check(result.IsSuccessStatusCode && result.Content.Headers.ContentType?.MediaType == "text/html"
                    && html.Contains("<!DOCTYPE html>") && html.Contains("/Contents/js/printThis.js")
                    && html.Contains("چاپ / ذخیره PDF")
                    && html.Contains(kind == "panel" ? "/Contents/css/MyStyle/PrintLabel.css"
                        : kind == "pack" ? "/Contents/css/MyStyle/PrintLA5.css" : "/Contents/css/MyStyle/PrintPA4.css"),
                    $"Delivery {kind} renders as an authenticated standalone legacy Razor page");
            }
            check((await browser.GetAsync("/print/orders/0/invoice")).StatusCode == HttpStatusCode.NotFound,
                "Invalid page document ID is rejected");
            check((await browser.GetAsync("/css/order-print.css")).IsSuccessStatusCode,
                "Print assets are available from the Web application");
            var largeTicket = cookies.Create(new() { AccessToken = new string('x', 9000), ExpiresAt = DateTimeOffset.UtcNow.AddHours(1) });
            var largeLogin = await browser.PostAsync("/print/session", Form(largeTicket));
            check(largeLogin.IsSuccessStatusCode && largeLogin.Headers.GetValues("Set-Cookie").Count() > 1,
                "Large role-bearing tokens use chunked cookies rather than exceeding browser limits");
            var context = new DefaultHttpContext();
            context.Request.Headers.Cookie = handler.CookieContainer.GetCookieHeader(new Uri(browser.BaseAddress!, "/print/orders/1/bill"));
            check(cookies.ReadRequest(context)?.AccessToken.Length == 9000, "Chunked print credentials are reconstructed correctly");
            await browser.PostAsync("/print/session", Form(null));
            check((await browser.GetAsync("/print/orders/1/invoice")).StatusCode == HttpStatusCode.Unauthorized,
                "Signing out removes print access including cookie chunks");
        }
        finally { await app.StopAsync(); }
    }

    private static FormUrlEncodedContent Form(string? ticket) => new(new Dictionary<string, string> { ["ticket"] = ticket ?? "" });
    private static OrderPrintData WebOrder(BaseSite.Api.Contracts.OrderPrintData value) =>
        JsonSerializer.Deserialize<OrderPrintData>(JsonSerializer.Serialize(value))
        ?? throw new InvalidOperationException("Print JSON contract could not be converted to the Web model.");

    private static string WebRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
            if (File.Exists(Path.Combine(dir.FullName, "BaseSite.Web", "BaseSite.Web.csproj"))) return Path.Combine(dir.FullName, "BaseSite.Web");
        throw new DirectoryNotFoundException();
    }
    internal static Order_Order Sample() => new()
    {
        Id = 1, DocNumber = 700001, StatusId = 1, DateOrder = new DateTime(2026, 9, 1),
        DateDelivery = new DateTime(2026, 9, 20), Cost = 1320000, SumCostAddition = 100000, Tax = 10, DeliveryCost = 0,
        ProjectName = "<script>injected</script>", Account_Users = new() { Name = "مشتری", LastName = "نمونه", EconomicalNumber = "123456789012" },
        Order_Cabin = [new() { Count = 2, Cost = 1000000, MonitorId = 1, CostMonitor = 100000,
            Tb_CabinPanels = new() { Id = 1, Name = "پنل نمونه", Order_ProductStatus = new() { Name = "مونتاژ" } },
            Tb_Monitors = new() { Name = "نمایشگر نمونه" }, PhoneCallButton = true,
            Order_Panel_Attachment = [new() { Count = 1, Cost = 100000, Tb_Attachments = new() { Id = 5, Name = "متعلقات" } }],
            Order_Panel_Addition = [new() { Cost = 100000, Tb_Additions = new() { Id = 1, Name = "اضافات" } }] }],
        Order_Hall = [new() { Count = 1, Cost = 100000, Tb_HallPanels = new() { Id = 1, Name = "پنل طبقات" } }],
        Order_DoorTop = [new() { Count = 1, Cost = 100000, Tb_DoorTopPanels = new() { Id = 1, Name = "پنل سردرب" } }]
    };

    internal sealed class FakePrintApi : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            if (request.Headers.Authorization?.ToString() != "Bearer print-test-token")
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            var parts = request.RequestUri!.AbsolutePath.Split('/');
            var id = int.Parse(parts[3]);
            if (id >= 400) return Task.FromResult(new HttpResponseMessage((HttpStatusCode)id));
            if (parts[2] == "payments")
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new PaymentPrintData
                    {
                        DocNumber = 9100001, Amount = 44000000, PrintedAt = "1405/06/26 10:30:00",
                        PaymentTypeName = "چک", ReferenceNumber = "123", DueDate = "1405/07/01",
                        BankName = "بانک نمونه", BankBranchCode = "101", AccountNumber = "1000",
                        Comment = "نمونه", CustomerName = "مشتری نمونه", PrinterName = "کاربر نمونه"
                    })
                });
            if (parts[2] == "deliveries")
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new DeliveryPrintData
                    {
                        DocNumber = 5700001, ShDate = "1405/06/26", RecieverName = "مشتری نمونه",
                        DestinationAddress = "تهران", Tb_PackTypes = new() { Name = "کارتن" },
                        Delivery_DeliveryLocations = new() { Name = "محل مشتری" },
                        Delivery_VehicleTypes = new() { Name = "وانت" },
                        Order_Order = new() { DocNumber = 700001, FactorNumber = 1001, ProjectName = "پروژه نمونه",
                            Account_Users = new() { FullName = "مشتری نمونه" } },
                        Items = [new() { Type = 1, Checked = true, Name = "پنل نمونه", Model = "مدل نمونه", Count = 1 }]
                    })
                });
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                { Content = JsonContent.Create(OrderPrintDataMapper.ForPrint(Sample(), parts.Last())) });
        }
    }
}
