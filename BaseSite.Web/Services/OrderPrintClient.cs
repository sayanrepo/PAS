using System.Net.Http.Headers;
using BaseSite.Web.Models;

namespace BaseSite.Web.Services;

public sealed class OrderPrintClient(HttpClient httpClient)
{
    public async Task<OrderPrintData> GetAsync(int id, string kind, string accessToken, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/orders/{id}/print/{kind}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderPrintData>(cancellationToken)
            ?? throw new HttpRequestException("Empty print response.");
    }
}
