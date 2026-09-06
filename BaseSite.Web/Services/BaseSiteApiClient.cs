using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BaseSite.Web.Models;

namespace BaseSite.Web.Services;

public sealed class BaseSiteApiClient(HttpClient httpClient, ApiSession session)
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorAsync(response, cancellationToken));

        return await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken)
            ?? throw new InvalidOperationException("پاسخ ورود از سرور معتبر نیست.");
    }

    public Task<DashboardSummary?> GetDashboardAsync(CancellationToken cancellationToken = default) =>
        GetAsync<DashboardSummary>("api/dashboard", cancellationToken);

    public Task<List<DocumentSummary>?> GetDocumentsAsync(
        string kind,
        int? documentNumber,
        string? customer,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>();
        if (documentNumber.HasValue) query.Add($"documentNumber={documentNumber.Value}");
        if (!string.IsNullOrWhiteSpace(customer)) query.Add($"customer={Uri.EscapeDataString(customer)}");
        var url = $"api/documents/{Uri.EscapeDataString(kind)}" + (query.Count == 0 ? string.Empty : "?" + string.Join("&", query));
        return GetAsync<List<DocumentSummary>>(url, cancellationToken);
    }

    public Task<List<CustomerSummary>?> GetCustomersAsync(string? term, CancellationToken cancellationToken = default)
    {
        var url = "api/customers" + (string.IsNullOrWhiteSpace(term) ? string.Empty : $"?term={Uri.EscapeDataString(term)}");
        return GetAsync<List<CustomerSummary>>(url, cancellationToken);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Post, "api/auth/logout");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        await session.SignOutAsync();
    }

    private async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken)
    {
        using var request = CreateRequest(HttpMethod.Get, url);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await session.SignOutAsync();
            return default;
        }
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorAsync(response, cancellationToken));

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        if (!string.IsNullOrWhiteSpace(session.AccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        return request;
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            if (json.RootElement.TryGetProperty("title", out var title))
                return title.GetString() ?? "خطا در ارتباط با سرور.";
        }
        catch (JsonException)
        {
        }

        return "خطا در ارتباط با سرور.";
    }
}
