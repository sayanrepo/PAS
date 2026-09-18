using System.Net.Http.Headers;
using BaseSite.Web.Models;

namespace BaseSite.Web.Services;

public sealed class SalePrintClient(HttpClient httpClient)
{
    public async Task<SalePrintData> GetAsync(int id, string kind, string accessToken, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/sales/{id}/print/{kind}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SalePrintData>(cancellationToken)
            ?? throw new HttpRequestException("Empty print response.");
    }
}
