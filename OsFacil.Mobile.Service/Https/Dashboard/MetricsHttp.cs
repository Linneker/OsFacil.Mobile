using OsFacil.Mobile.Service.Https.Clients.Response;
using OsFacil.Mobile.Service.Https.Dashboard.Response;
using OsFacil.Mobile.Service.Util;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace OsFacil.Mobile.Service.Https.Dashboard;

public sealed class MetricsHttp : IMetricsHttp
{
    private readonly HttpClient _httpClient;

    public MetricsHttp(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResponseHttps<DashboardMetricsHttpResponse>> GetDashboardAsync(string accessToken, CancellationToken ct = default)
    {
        var responseHttps = new ResponseHttps<DashboardMetricsHttpResponse>();
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await _httpClient.GetAsync("/api/metrics/dashboard", cancellationToken: ct);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);

            // Aqui você decide: lançar, retornar erro no response, etc.
            // Vou lançar com detalhes pra você enxergar o problema no debug.
            responseHttps.IsSuccessStatusCode = false;
            responseHttps.StatusCode = (int)response.StatusCode;
            responseHttps.Error = string.IsNullOrEmpty(errorBody) ? "Sem informações" : errorBody;
        }
        else
        {
            var result = await response.Content.ReadFromJsonAsync<DashboardMetricsHttpResponse>(
    options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
    cancellationToken: ct);
            responseHttps.Data = result ?? new();
            responseHttps.IsSuccessStatusCode = true;
            responseHttps.StatusCode = (int)response.StatusCode;
        }
        return responseHttps;
    }
}
