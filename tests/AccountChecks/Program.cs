using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Text.Json;
using BaseSite.Web.Models;
using BaseSite.Web.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.JSInterop;

var count = 0;
void Check(bool condition, string description)
{
    if (!condition) throw new Exception(description);
    count++;
    Console.WriteLine($"PASS: {description}");
}

var model = new ChangePasswordRequest
{
    UserName = "account-test", CurrentPassword = "old-test", NewPassword = "new-test", ConfirmPassword = "different"
};
var errors = new List<ValidationResult>();
Check(!Validator.TryValidateObject(model, new ValidationContext(model), errors, true), "Mismatched password confirmation is rejected");
model.ConfirmPassword = model.NewPassword;
Check(Validator.TryValidateObject(model, new ValidationContext(model), new List<ValidationResult>(), true), "Valid account change passes validation");
Check(!JsonSerializer.Serialize(model).Contains("ConfirmPassword"), "Confirmation is not sent to the API");
model.UserName = new string('x', 256);
Check(!Validator.TryValidateObject(model, new ValidationContext(model), new List<ValidationResult>(), true), "Oversized username is rejected");
model.UserName = "account-test";
model.NewPassword = model.ConfirmPassword = "abc";
Check(!Validator.TryValidateObject(model, new ValidationContext(model), new List<ValidationResult>(), true), "Short password is rejected");

foreach (var invalidImage in new[] { Array.Empty<byte>(), Encoding.UTF8.GetBytes("<svg xmlns='http://www.w3.org/2000/svg'></svg>"), new byte[ProfileImageStore.MaxBytes + 1] })
{
    var rejected = false;
    try { ProfileImageStore.GetExtension(invalidImage); }
    catch (InvalidOperationException) { rejected = true; }
    Check(rejected, "Empty, active-content, or oversized image is rejected");
}
var profileImage = Path.GetFullPath("BaseSite.Web/wwwroot/Images/System/profile.png");
Check(ProfileImageStore.GetExtension(await File.ReadAllBytesAsync(profileImage)) == "png", "Existing profile image is supported");

var js = new StorageJs();
var session = new ApiSession(new ProtectedLocalStorage(js, new EphemeralDataProtectionProvider()));
await session.SignInAsync(new LoginResponse { AccessToken = "test-token", ExpiresAt = DateTimeOffset.UtcNow.AddHours(1), User = new CurrentUser { Id = 17 } });
var handler = new ResponseHandler();
var api = new BaseSiteApiClient(new HttpClient(handler) { BaseAddress = new Uri("http://account-test.invalid/") }, session);
handler.Status = HttpStatusCode.BadRequest;
handler.Body = "{\"title\":\"رمز عبور فعلی اشتباه است\"}";
string? message = null;
try { await api.ChangePasswordAsync(model); }
catch (InvalidOperationException ex) { message = ex.Message; }
Check(message == "رمز عبور فعلی اشتباه است", "API rejection reaches the form instead of reporting success");
Check(session.AccessToken == "test-token", "Rejected change preserves the current session");
Check(handler.Authorization == "Bearer test-token", "Account updates carry the authenticated user's bearer token");
handler.Status = HttpStatusCode.OK;
handler.Body = JsonSerializer.Serialize(new LoginResponse { AccessToken = "updated-token", ExpiresAt = DateTimeOffset.UtcNow.AddHours(1), User = new CurrentUser { Id = 17, UserName = "renamed", ImagePath = "17_test.png" } });
var response = await api.ChangeImageAsync("17_test.png");
await session.SignInAsync(response);
Check(session.User!.UserName == "renamed" && session.User.ImagePath == "17_test.png", "Updated identity and avatar replace the active session");
// SignIn persists encrypted storage; the same storage/provider also survives a new session instance.
var provider = new EphemeralDataProtectionProvider();
var persistedSession = new ApiSession(new ProtectedLocalStorage(js, provider));
await persistedSession.SignInAsync(response);
var restored = new ApiSession(new ProtectedLocalStorage(js, provider));
await restored.InitializeAsync();
Check(restored.User?.ImagePath == "17_test.png" && restored.User.UserName == "renamed", "Avatar and username survive session restoration");
handler.Status = HttpStatusCode.Unauthorized;
handler.Body = "{}";
try { await api.ChangeImageAsync("17_test.png"); } catch (InvalidOperationException) { }
Check(!session.IsAuthenticated && session.User is null, "Expired authentication clears the session");

var controller = new BaseSite.Api.Controllers.AuthenticationController(
    new BaseSite.Api.Authentication.AccessTokenService(provider, new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build()))
{
    ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
    {
        HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
        {
            User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(
                [new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "17")], "Test"))
        }
    }
};
foreach (var path in new[] { "../profile.png", "18_0123456789abcdef0123456789abcdef.png", "17_0123456789abcdef0123456789abcdef.svg", "" })
    Check(controller.ChangeImage(new BaseSite.Api.Contracts.ChangeImageRequest { ImagePath = path }) is Microsoft.AspNetCore.Mvc.BadRequestObjectResult,
        "API rejects traversal, another user's filename, unsupported extension, or empty image path");
Check(!BaseSite.Models.Account.AccountManager.Account_User_TryChangePassword(17, " ", "test", "test", out _), "Blank username is rejected before database access");
Console.WriteLine($"{count} account checks passed.");

sealed class ResponseHandler : HttpMessageHandler
{
    public HttpStatusCode Status { get; set; }
    public string Body { get; set; } = "{}";
    public string? Authorization { get; private set; }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Authorization = request.Headers.Authorization?.ToString();
        return Task.FromResult(new HttpResponseMessage(Status) { Content = new StringContent(Body, Encoding.UTF8, "application/json") });
    }
}

sealed class StorageJs : IJSRuntime
{
    private string? value;
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => InvokeAsync<TValue>(identifier, CancellationToken.None, args);
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        if (identifier == "localStorage.setItem") value = (string?)args![1];
        if (identifier == "localStorage.removeItem") value = null;
        return ValueTask.FromResult(identifier == "localStorage.getItem" ? (TValue)(object?)value! : default!);
    }
}
