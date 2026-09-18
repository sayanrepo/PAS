using System.Net.Http.Headers;
using BaseSite.Web.Models;

namespace BaseSite.Web.Services;

public sealed class ServicePrintClient(HttpClient httpClient)
{
    public async Task<ServicePrintData> GetAsync(int id, string kind, string accessToken, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/services/{id}/print/{kind}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ServicePrintData>(cancellationToken)
            ?? throw new HttpRequestException("Empty print response.");
    }
}
