#nullable enable

using System.Security.Claims;
using System.Text.Json;
using BaseSite.Api.Contracts;
using Microsoft.AspNetCore.DataProtection;

namespace BaseSite.Api.Authentication;

public sealed class AccessTokenService(IDataProtectionProvider dataProtectionProvider, IConfiguration configuration)
{
    private readonly IDataProtector protector = dataProtectionProvider.CreateProtector("BaseSite.Api.AccessToken.v1");
    private readonly TimeSpan lifetime = TimeSpan.FromMinutes(configuration.GetValue("Authentication:AccessTokenMinutes", 480));

    public LoginResponse Create(CurrentUserDto user)
    {
        var expiresAt = DateTimeOffset.UtcNow.Add(lifetime);
        var payload = JsonSerializer.Serialize(new AccessTokenPayload { User = user, ExpiresAt = expiresAt });
        return new LoginResponse
        {
            AccessToken = protector.Protect(payload),
            ExpiresAt = expiresAt,
            User = user
        };
    }

    public ClaimsPrincipal? Validate(string token)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<AccessTokenPayload>(protector.Unprotect(token));
            if (payload?.User is null || payload.ExpiresAt <= DateTimeOffset.UtcNow)
                return null;

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, payload.User.Id.ToString()),
                new(ClaimTypes.Name, payload.User.UserName),
                new("panta:full_name", payload.User.FullName),
                new("panta:image_path", payload.User.ImagePath)
            };
            claims.AddRange(payload.User.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
            return new ClaimsPrincipal(new ClaimsIdentity(claims, AccessTokenAuthenticationHandler.SchemeName));
        }
        catch
        {
            return null;
        }
    }

    private sealed class AccessTokenPayload
    {
        public CurrentUserDto User { get; set; } = new();
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
