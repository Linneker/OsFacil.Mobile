using OsFacil.Mobile.Service.Util;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace OsFacil.Mobile.Service.Https.Billing;

public class BillingHttp : IBillingHttp
{
    private readonly HttpClient _httpClient;

    public BillingHttp(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResponseHttps<BillingStatusResponse>> GetStatusAsync(string token, CancellationToken ct = default)
    {
        var responseHttps = new ResponseHttps<BillingStatusResponse>();
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await _httpClient.GetAsync("/api/billing/me", ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                responseHttps.IsSuccessStatusCode = false;
                responseHttps.StatusCode = (int)response.StatusCode;
                responseHttps.Error = string.IsNullOrEmpty(errorBody) ? "Erro ao verificar assinatura" : errorBody;
            }
            else
            {
                var result = await response.Content.ReadFromJsonAsync<BillingStatusResponse>(
                    options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                    cancellationToken: ct);
                responseHttps.Data = result ?? new BillingStatusResponse();
                responseHttps.IsSuccessStatusCode = true;
                responseHttps.StatusCode = (int)response.StatusCode;
            }
        }
        catch (Exception e)
        {
            responseHttps.IsSuccessStatusCode = false;
            responseHttps.StatusCode = 500;
            responseHttps.Error = $"Erro ao verificar assinatura: {e.Message}";
        }
        return responseHttps;
    }

    public async Task<ResponseHttps<BillingInvoiceResponse>> GetInvoiceAsync(string token, Guid invoiceId, CancellationToken ct = default)
    {
        var responseHttps = new ResponseHttps<BillingInvoiceResponse>();
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await _httpClient.GetAsync($"/api/billing/invoices/{invoiceId:D}", ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                responseHttps.IsSuccessStatusCode = false;
                responseHttps.StatusCode = (int)response.StatusCode;
                responseHttps.Error = string.IsNullOrEmpty(errorBody) ? "Erro ao consultar invoice" : errorBody;
            }
            else
            {
                var result = await response.Content.ReadFromJsonAsync<BillingInvoiceResponse>(
                    options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                    cancellationToken: ct);
                responseHttps.Data = result ?? new BillingInvoiceResponse();
                responseHttps.IsSuccessStatusCode = true;
                responseHttps.StatusCode = (int)response.StatusCode;
            }
        }
        catch (Exception e)
        {
            responseHttps.IsSuccessStatusCode = false;
            responseHttps.StatusCode = 500;
            responseHttps.Error = $"Erro ao consultar invoice: {e.Message}";
        }
        return responseHttps;
    }

    public async Task<ResponseHttps<RenewResponse>> RenewAsync(string token, RenewRequest request, CancellationToken ct = default)
    {
        var responseHttps = new ResponseHttps<RenewResponse>();
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await _httpClient.PostAsJsonAsync("/api/billing/renew", request, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                responseHttps.IsSuccessStatusCode = false;
                responseHttps.StatusCode = (int)response.StatusCode;
                responseHttps.Error = string.IsNullOrEmpty(errorBody) ? "Erro ao renovar assinatura" : errorBody;
            }
            else
            {
                var result = await response.Content.ReadFromJsonAsync<RenewResponse>(
                    options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                    cancellationToken: ct);
                responseHttps.Data = result ?? new RenewResponse();
                responseHttps.IsSuccessStatusCode = true;
                responseHttps.StatusCode = (int)response.StatusCode;
            }
        }
        catch (Exception e)
        {
            responseHttps.IsSuccessStatusCode = false;
            responseHttps.StatusCode = 500;
            responseHttps.Error = $"Erro ao renovar assinatura: {e.Message}";
        }
        return responseHttps;
    }
}
