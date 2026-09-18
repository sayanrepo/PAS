using System.Net;
using BaseSite.Web.Models;
using BaseSite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaseSite.Web.Pages.Print.Orders;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public abstract class OrderPrintPageModel(OrderPrintClient client, PrintSessionCookie cookies) : PageModel
{
    protected abstract string Kind { get; }
    public OrderPrintData Order { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        Response.Headers["Referrer-Policy"] = "no-referrer";
        if (id <= 0) return Error(404, "سفارش پیدا نشد.");
        var credential = cookies.ReadRequest(HttpContext);
        if (credential is null)
            return Error(401, "نشست شما پایان یافته است. وارد برنامه شوید و صفحه چاپ را دوباره باز کنید.");
        try
        {
            Order = await client.GetAsync(id, Kind, credential.AccessToken, cancellationToken);
            OrderPrintPresentation.Prepare(Order, Kind);
            return Page();
        }
        catch (HttpRequestException ex)
        {
            return ex.StatusCode switch
            {
                HttpStatusCode.Unauthorized => Error(401, "نشست شما پایان یافته است. دوباره وارد برنامه شوید."),
                HttpStatusCode.Forbidden => Error(403, "شما دسترسی لازم برای چاپ این سند را ندارید."),
                HttpStatusCode.NotFound => Error(404, "سفارش پیدا نشد."),
                _ => Error(502, "دریافت اطلاعات چاپ از سرور ناموفق بود. دوباره تلاش کنید.")
            };
        }
        catch (System.Text.Json.JsonException)
        {
            return Error(502, "اطلاعات چاپ دریافت‌شده از سرور معتبر نیست.");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return Error(504, "زمان دریافت اطلاعات چاپ به پایان رسید. دوباره تلاش کنید.");
        }
    }

    private static ContentResult Error(int status, string message) => new()
    {
        StatusCode = status, ContentType = "text/plain; charset=utf-8", Content = message
    };
}

public sealed class SpecificationModel(OrderPrintClient client, PrintSessionCookie cookies)
    : OrderPrintPageModel(client, cookies)
{
    protected override string Kind => "specification";
}

public sealed class InvoiceModel(OrderPrintClient client, PrintSessionCookie cookies)
    : OrderPrintPageModel(client, cookies)
{
    protected override string Kind => "invoice";
}

public sealed class BillModel(OrderPrintClient client, PrintSessionCookie cookies)
    : OrderPrintPageModel(client, cookies)
{
    protected override string Kind => "bill";
}
