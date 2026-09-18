using System.Net;
using BaseSite.Web.Models;
using BaseSite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaseSite.Web.Pages.Print.Sales;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public abstract class SalePrintPageModel(SalePrintClient client, PrintSessionCookie cookies) : PageModel
{
    protected abstract string Kind { get; }
    public SalePrintData Sale { get; private set; } = new();
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        Response.Headers["Referrer-Policy"] = "no-referrer";
        if (id <= 0) return Error(404, "فروش کالا پیدا نشد.");
        var credential = cookies.ReadRequest(HttpContext);
        if (credential is null) return Error(401, "نشست شما پایان یافته است. وارد برنامه شوید و صفحه چاپ را دوباره باز کنید.");
        try { Sale = await client.GetAsync(id, Kind, credential.AccessToken, cancellationToken); return Page(); }
        catch (HttpRequestException ex)
        {
            return ex.StatusCode switch
            {
                HttpStatusCode.Unauthorized => Error(401, "نشست شما پایان یافته است. دوباره وارد برنامه شوید."),
                HttpStatusCode.Forbidden => Error(403, "شما دسترسی لازم برای چاپ این سند را ندارید."),
                HttpStatusCode.NotFound => Error(404, "فروش کالا پیدا نشد."),
                _ => Error(502, "دریافت اطلاعات چاپ از سرور ناموفق بود. دوباره تلاش کنید.")
            };
        }
        catch (System.Text.Json.JsonException) { return Error(502, "اطلاعات چاپ دریافت‌شده از سرور معتبر نیست."); }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) { return Error(504, "زمان دریافت اطلاعات چاپ به پایان رسید. دوباره تلاش کنید."); }
    }
    private static ContentResult Error(int status, string message) => new() { StatusCode = status, ContentType = "text/plain; charset=utf-8", Content = message };
}

public sealed class BillModel(SalePrintClient client, PrintSessionCookie cookies) : SalePrintPageModel(client, cookies) { protected override string Kind => "bill"; }
public sealed class InvoiceModel(SalePrintClient client, PrintSessionCookie cookies) : SalePrintPageModel(client, cookies) { protected override string Kind => "invoice"; }
