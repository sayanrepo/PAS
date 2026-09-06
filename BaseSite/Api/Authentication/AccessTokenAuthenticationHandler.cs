#nullable enable

using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace BaseSite.Api.Authentication;

public sealed class AccessTokenAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    AccessTokenService accessTokens)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "BaseSiteBearer";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorization = Request.Headers.Authorization.ToString();
        if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.NoResult());

        var token = authorization["Bearer ".Length..].Trim();
        var principal = accessTokens.Validate(token);
        return Task.FromResult(principal is null
            ? AuthenticateResult.Fail("توکن دسترسی نامعتبر یا منقضی شده است.")
            : AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
    }
}
