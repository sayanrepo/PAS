using BaseSite.Models;
using BaseSite.Models.Account;
using BaseSite.Models.DBModel;
using System.Security.Claims;

namespace BaseSite.Infrastructure;

public static class AuthenticationClaims
{
    public const string FullName = "panta:full_name";
    public const string ImagePath = "panta:image_path";

    public static async Task SignInAsync(HttpContext context, Account_Users user)
    {
        var operations = user.Account_UserPost.Count > 0
            ? AccountManager.Account_Operation_Get((AccountRole)user.Account_UserPost.First().PostId)
            : new List<OPERATIONS>();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(FullName, user.FullName ?? string.Empty),
            new(ImagePath, user.ImagePath ?? "profile.png")
        };
        claims.AddRange(operations.Distinct().Select(operation => new Claim(ClaimTypes.Role, operation.ToString())));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = false, AllowRefresh = true });
    }
}

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal) =>
        int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    public static string GetUserName(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

    public static string GetFullName(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(AuthenticationClaims.FullName) ?? string.Empty;

    public static string GetImagePath(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(AuthenticationClaims.ImagePath) ?? "profile.png";

    public static IReadOnlyCollection<OPERATIONS> GetOperations(this ClaimsPrincipal principal) =>
        principal.FindAll(ClaimTypes.Role)
            .Select(claim => Enum.TryParse<OPERATIONS>(claim.Value, out var operation) ? operation : (OPERATIONS?)null)
            .Where(operation => operation.HasValue)
            .Select(operation => operation.Value)
            .ToArray();
}
