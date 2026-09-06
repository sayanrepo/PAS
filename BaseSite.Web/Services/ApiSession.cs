using BaseSite.Web.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace BaseSite.Web.Services;

public sealed class ApiSession(ProtectedLocalStorage storage)
{
    private const string StorageKey = "basesite.session";

    public LoginResponse? Login { get; private set; }
    public CurrentUser? User => Login?.User;
    public string? AccessToken => Login?.AccessToken;
    public bool IsAuthenticated => Login is not null && Login.ExpiresAt > DateTimeOffset.UtcNow;
    public bool IsInitialized { get; private set; }

    public event Action? Changed;

    public async Task InitializeAsync()
    {
        if (IsInitialized)
            return;

        try
        {
            var result = await storage.GetAsync<LoginResponse>(StorageKey);
            Login = result.Success && result.Value?.ExpiresAt > DateTimeOffset.UtcNow ? result.Value : null;
        }
        catch (InvalidOperationException)
        {
            return;
        }
        catch (JSDisconnectedException)
        {
            return;
        }

        IsInitialized = true;
        Changed?.Invoke();
    }

    public async Task SignInAsync(LoginResponse login)
    {
        Login = login;
        IsInitialized = true;
        await storage.SetAsync(StorageKey, login);
        Changed?.Invoke();
    }

    public async Task SignOutAsync()
    {
        Login = null;
        IsInitialized = true;
        await storage.DeleteAsync(StorageKey);
        Changed?.Invoke();
    }
}
