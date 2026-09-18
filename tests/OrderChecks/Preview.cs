using BaseSite.Api.Contracts;
using BaseSite.Api.Queries;
using BaseSite.Web.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using MudBlazor.Services;
using System.Net;
using System.Net.Http.Json;
using System.Globalization;

internal static class Preview
{
    public static async Task RunAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args.Where(x => x != "--preview").ToArray(),
            ContentRootPath = FindWebRoot(), EnvironmentName = "Development", WebRootPath = "wwwroot"
        });
        builder.WebHost.UseSetting(WebHostDefaults.StaticWebAssetsKey, Path.Combine(AppContext.BaseDirectory, "BaseSite.Web.staticwebassets.runtime.json"));
        builder.WebHost.UseStaticWebAssets();
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddMudServices();
        builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
        builder.Services.AddSingleton<IDataProtectionProvider>(new EphemeralDataProtectionProvider());
        builder.Services.AddScoped(_ =>
        {
            // An isolated test identity; never connects to the real API or database.
            var session = new ApiSession(new ProtectedLocalStorage(new EmptyStorage(), new EphemeralDataProtectionProvider()), new EmptyStorage(), new PrintSessionCookie(new EphemeralDataProtectionProvider()));
            session.SignInAsync(new BaseSite.Web.Models.LoginResponse
            {
                AccessToken = "preview-only", ExpiresAt = DateTimeOffset.UtcNow.AddHours(1),
                User = new() { Id = 1, FullName = "کاربر آزمایشی", Roles = ["Order", "Order_Search", "Order_Detail", "Order_Add", "Order_Edit_Factor", "Plan_Print", "Order_Print", "Logs_Detail"] }
            }).GetAwaiter().GetResult();
            return session;
        });
        builder.Services.AddScoped(sp => new BaseSiteApiClient(new HttpClient(new SampleApi())
            { BaseAddress = new Uri("http://orders-preview.invalid/") }, sp.GetRequiredService<ApiSession>()));
        var app = builder.Build();
        app.UseStaticFiles();
        app.UseAntiforgery();
        app.MapStaticAssets(Path.Combine(AppContext.BaseDirectory, "BaseSite.Web.staticwebassets.endpoints.json"));
        app.MapRazorComponents<OrderChecks.PreviewApp>().AddInteractiveServerRenderMode()
            .AddAdditionalAssemblies(typeof(BaseSite.Web.Components.Routes).Assembly);
        await app.RunAsync("http://localhost:5089");
    }

    private static string FindWebRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
            if (Directory.Exists(Path.Combine(directory.FullName, "BaseSite.Web", "wwwroot")))
                return Path.Combine(directory.FullName, "BaseSite.Web");
        throw new DirectoryNotFoundException("BaseSite.Web was not found.");
    }

    private sealed class EmptyStorage : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => ValueTask.FromResult(default(TValue)!);
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => ValueTask.FromResult(default(TValue)!);
    }

    private sealed class SampleApi : HttpMessageHandler
    {
        private readonly Dictionary<int, OrderEditor> editors = [];
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            var path = request.RequestUri!.AbsolutePath;
            var query = QueryHelpers.ParseQuery(request.RequestUri.Query);
            string? Get(string key) => query.TryGetValue(key, out var value) ? value.ToString() : null;
            int? Int(string key) => int.TryParse(Get(key), out var value) ? value : null;
            DateTime? Date(string key) => DateTime.TryParse(Get(key), CultureInfo.InvariantCulture, DateTimeStyles.None, out var value) ? value : null;
            object data;
            if (path == "/api/orders/editor/customers") data = new[] { new OrderLookup { Id = 1, Name = "مشتری نمونه" } };
            else if (path == "/api/orders/editor/new") data = SampleOrderEditor.Create(0);
            else if (path == "/api/orders/editor" || path.StartsWith("/api/orders/editor/"))
            {
                var editorId = int.TryParse(path.Split('/').Last(), out var selectedId) ? selectedId : 100;
                if (!editors.TryGetValue(editorId, out var editor)) editor = SampleOrderEditor.Create(editorId);
                if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
                {
                    if (!editor.CanEdit) return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict));
                    editor.Form = request.Content!.ReadFromJsonAsync<OrderForm>(token).GetAwaiter().GetResult()!;
                    editor.Detail.Summary.ProjectName = editor.Form.ProjectName;
                    editors[editorId] = editor;
                    data = new CreatedDocument { Id = editorId, DocumentNumber = editor.Detail.Summary.DocumentNumber };
                }
                else data = editor;
            }
            else if (path.EndsWith("/activity/comments", StringComparison.Ordinal) && request.Method == HttpMethod.Post)
            {
                var comment = request.Content!.ReadFromJsonAsync<BaseSite.Web.Models.OrderCommentRequest>(token).GetAwaiter().GetResult()!;
                data = new BaseSite.Web.Models.OrderCommentItem { Id = 3, Owner = "کاربر آزمایشی", Comment = comment.Comment, CreatedAt = DateTime.Now };
            }
            else if (path.EndsWith("/activity", StringComparison.Ordinal))
            {
                data = new BaseSite.Web.Models.OrderActivity
                {
                    Comments =
                    [
                        new() { Id = 1, Owner = "مدیر فروش", Comment = "زمان تحویل با مشتری هماهنگ شد.", CreatedAt = new DateTime(2026, 9, 15, 9, 20, 0) },
                        new() { Id = 2, Owner = "کاربر آزمایشی", Comment = "رنگ پنل مطابق نمونه تأیید شده است.", CreatedAt = new DateTime(2026, 9, 16, 11, 45, 0) }
                    ],
                    History =
                    [
                        new() { Id = 12, EventTime = new DateTime(2026, 9, 16, 10, 12, 0), User = "کاربر آزمایشی", Category = "سفارش", DocumentNumber = 1042, Activity = "ویرایش", Description = "اطلاعات سفارش ویرایش شد.", IpAddress = "192.168.1.24", Amount = 248500000 },
                        new() { Id = 11, EventTime = new DateTime(2026, 9, 14, 8, 30, 0), User = "مدیر فروش", Category = "سفارش", DocumentNumber = 1042, Activity = "ایجاد", Description = "سفارش ایجاد شد.", IpAddress = "192.168.1.10" }
                    ]
                };
            }
            else if (path.Contains("/print/", StringComparison.Ordinal))
            {
                var title = path.EndsWith("/specification", StringComparison.Ordinal) ? "کارت مشخصات سفارش" : path.EndsWith("/bill", StringComparison.Ordinal) ? "صورتحساب فروش" : "فاکتور";
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent($"<!doctype html><html lang=\"fa\" dir=\"rtl\"><head><meta charset=\"utf-8\"><title>{title}</title></head><body><h1>{title}</h1></body></html>", System.Text.Encoding.UTF8, "text/html")
                });
            }
            else if (path == "/api/orders/lookups") data = new OrderLookups
            {
                Statuses = [new() { Id = 1, Name = "پیش فاکتور" }, new() { Id = 2, Name = "در جریان تولید" }],
                TradeTypes = [new() { Id = 1, Name = "فروش" }]
            };
            else if (path == "/api/orders/customers") data = new[] { new OrderLookup { Id = 1, Name = "مشتری نمونه" } };
            else if (path == "/api/orders")
            {
                var filter = new OrderSearch
                {
                    DocumentNumber = Int("documentNumber"), CustomerId = Int("customerId"), Customer = Get("customer"),
                    StatusId = (byte?)Int("statusId"), TradeTypeId = (byte?)Int("tradeTypeId"), ProjectName = Get("projectName"),
                    OrderDateFrom = Date("orderDateFrom"), OrderDateTo = Date("orderDateTo"), FactorDateFrom = Date("factorDateFrom"), FactorDateTo = Date("factorDateTo"),
                    Page = Int("page") ?? 0, PageSize = Int("pageSize") ?? 20
                };
                var rows = OrderQueries.Apply(SampleOrders.Create(), filter).OrderByDescending(x => x.Id);
                var items = OrderQueries.Project(rows.Skip(filter.Page * filter.PageSize).Take(filter.PageSize)).ToList();
                for (int i = 0; i < items.Count; i++) items[i].RowNumber = filter.Page * filter.PageSize + i + 1;
                data = new OrderPage { Items = items, TotalCount = rows.Count() };
            }
            else if (path.StartsWith("/api/orders/") && int.TryParse(path.Split('/').Last(), out var id))
            {
                var order = SampleOrders.Create().FirstOrDefault(x => x.Id == id);
                if (order is null) return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = JsonContent.Create(new { title = "سفارش یافت نشد." }) });
                data = OrderDetails.Map(order);
            }
            else return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(data) });
        }
    }
}
