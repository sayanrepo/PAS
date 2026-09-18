using System.Security.Cryptography;
using System.Text.Json;
using BaseSite.Web.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BaseSite.Web.Services;

// Bridge the existing Blazor login to independent HTTP requests without exposing
// a bearer token in a URL or relying on the lifetime of a circuit.
public sealed class PrintSessionCookie(IDataProtectionProvider protection)
{
    public const string Name = "BaseSite.PrintSession";
    private readonly IDataProtector protector = protection.CreateProtector("BaseSite.Web.PrintSession.v1");
    private readonly ChunkingCookieManager cookieManager = new();

    public PrintCredential? ReadRequest(HttpContext context) => Read(cookieManager.GetRequestCookie(context, Name));

    public void Write(HttpContext context, string ticket, DateTimeOffset expires) =>
        cookieManager.AppendResponseCookie(context, Name, ticket, Options(context.Request, expires));

    public void Delete(HttpContext context) =>
        cookieManager.DeleteCookie(context, Name, Options(context.Request));

    public string Create(LoginResponse login) => protector.Protect(JsonSerializer.Serialize(
        new PrintCredential(login.AccessToken, login.ExpiresAt)));

    public PrintCredential? Read(string? ticket)
    {
        if (string.IsNullOrWhiteSpace(ticket)) return null;
        try
        {
            var credential = JsonSerializer.Deserialize<PrintCredential>(protector.Unprotect(ticket));
            return credential is { AccessToken.Length: > 0 } && credential.ExpiresAt > DateTimeOffset.UtcNow
                ? credential : null;
        }
        catch (CryptographicException) { return null; }
        catch (JsonException) { return null; }
    }

    public static CookieOptions Options(HttpRequest request, DateTimeOffset? expires = null) => new()
    {
        HttpOnly = true,
        Secure = request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Path = request.PathBase.Add("/print").Value,
        Expires = expires,
        IsEssential = true
    };
}

public sealed record PrintCredential(string AccessToken, DateTimeOffset ExpiresAt);
