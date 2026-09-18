using BaseSite.Web.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaseSite.Web.Pages.Print;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class SessionModel(PrintSessionCookie cookies, IAntiforgery antiforgery) : PageModel
{
    public IActionResult OnGet() => new JsonResult(new
    {
        requestToken = antiforgery.GetAndStoreTokens(HttpContext).RequestToken
    });

    // Razor Pages validates the antiforgery header before entering this handler.
    public IActionResult OnPost(string? ticket)
    {
        if (string.IsNullOrEmpty(ticket))
        {
            cookies.Delete(HttpContext);
            return new NoContentResult();
        }
        var credential = cookies.Read(ticket);
        if (credential is null) return Unauthorized();
        cookies.Write(HttpContext, ticket, credential.ExpiresAt);
        return new NoContentResult();
    }
}
