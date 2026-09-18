using System.Net;
using BaseSite.Web.Models;
using BaseSite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaseSite.Web.Pages.Print.Payments;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class ReceiptModel(PaymentPrintClient client, PrintSessionCookie cookies) : PageModel
{
    public PaymentPrintData Payment { get; private set; } = new();
    public bool Accounting { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id, string kind, CancellationToken cancellationToken)
    {
        Response.Headers["Referrer-Policy"] = "no-referrer";
        if (id <= 0 || kind is not ("accounting" or "customer")) return Error(404, "سند دریافتی پیدا نشد.");
        var credential = cookies.ReadRequest(HttpContext);
        if (credential is null) return Error(401, "نشست شما پایان یافته است. وارد برنامه شوید و صفحه چاپ را دوباره باز کنید.");
        try
        {
            Payment = await client.GetAsync(id, kind, credential.AccessToken, cancellationToken);
            Accounting = kind == "accounting";
            return Page();
        }
        catch (HttpRequestException ex)
        {
            return ex.StatusCode switch
            {
                HttpStatusCode.Unauthorized => Error(401, "نشست شما پایان یافته است. دوباره وارد برنامه شوید."),
                HttpStatusCode.Forbidden => Error(403, "شما دسترسی لازم برای چاپ این سند را ندارید."),
                HttpStatusCode.NotFound => Error(404, "سند دریافتی پیدا نشد."),
                _ => Error(502, "دریافت اطلاعات چاپ از سرور ناموفق بود. دوباره تلاش کنید.")
            };
        }
        catch (System.Text.Json.JsonException) { return Error(502, "اطلاعات چاپ دریافت‌شده از سرور معتبر نیست."); }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) { return Error(504, "زمان دریافت اطلاعات چاپ به پایان رسید. دوباره تلاش کنید."); }
    }

    private static ContentResult Error(int status, string message) => new()
        { StatusCode = status, ContentType = "text/plain; charset=utf-8", Content = message };
}
