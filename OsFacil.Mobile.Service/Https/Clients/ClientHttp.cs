using OsFacil.Mobile.Service.Https.Clients.Request;
using OsFacil.Mobile.Service.Https.Clients.Response;
using OsFacil.Mobile.Service.Util;
using System.Net.Http.Json;
using System.Text.Json;

namespace OsFacil.Mobile.Service.Https.Clients;

public class ClientHttp : IClientHttp
{
    private readonly HttpClient _httpClient;

    public ClientHttp(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResponseHttps<CreateClientHttpResponse>> CreateClientAsync(string token, CreateClientHttpRequest request, CancellationToken ct = default)
    {
        ResponseHttps<CreateClientHttpResponse> responseHttps = new ResponseHttps<CreateClientHttpResponse>();
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            using var response = await _httpClient.PostAsJsonAsync($"/api/clients", request,
    cancellationToken: ct);

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
                var result = await response.Content.ReadFromJsonAsync<CreateClientHttpResponse>(
        options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
        cancellationToken: ct);
                if (result is null)
                {
                    responseHttps.IsSuccessStatusCode = false;
                    responseHttps.StatusCode = (int)response.StatusCode;
                    responseHttps.Error = "Cliente não registrado";
                }
                else
                {
                    responseHttps.Data = result;
                    responseHttps.IsSuccessStatusCode = true;
                    responseHttps.StatusCode = (int)response.StatusCode;
                }
            }

        }
        catch (Exception e)
        {
            responseHttps.IsSuccessStatusCode = false;
            responseHttps.StatusCode = 500;
            responseHttps.Error = $"Erro ao recuperar informações {e.Message}";
        }
        return responseHttps;

    }

    public async Task<ResponseHttps<DeleteHttpResponse>> DeleteClientsAsync(string token, string id, string name, CancellationToken ct = default)
    {
        var responsesHttps = new ResponseHttps<DeleteHttpResponse>();
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        using var response = await _httpClient.DeleteAsync($"/api/clients/{id}",
cancellationToken: ct);

        if (!response.IsSuccessStatusCode)
        {
            responsesHttps.IsSuccessStatusCode = false;
            responsesHttps.StatusCode = (int)response.StatusCode;
            responsesHttps.Error = $"Não foi possível remover o cliente {name}, tente novamente.";
        }
        else
        {
            responsesHttps.Data = new($"Cliente {name} removido com sucesso");
            responsesHttps.IsSuccessStatusCode = true;
            responsesHttps.StatusCode = (int)response.StatusCode;
           
        }
        return responsesHttps;
    }

    public async Task<ResponsesHttps<ClientHttpResponse>> GetClientsAsync(string token, string search, int page, int pageSize, CancellationToken ct = default)
    {
        ResponsesHttps<ClientHttpResponse> responseHttps = new ResponsesHttps<ClientHttpResponse>();
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            using var response = await _httpClient.GetAsync($"/api/clients/paginated?search={search}&PageNumber={page}&PageSize={pageSize}",
    cancellationToken: ct);

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
                var result = await response.Content.ReadFromJsonAsync<PagedResponse<ClientHttpResponse>>(
        options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
        cancellationToken: ct);
                responseHttps.Data = result?.Items.ToList() ?? [];
                responseHttps.IsSuccessStatusCode = true;
                responseHttps.StatusCode = (int)response.StatusCode;
            }

        }
        catch (Exception e)
        {
            responseHttps.IsSuccessStatusCode = false;
            responseHttps.StatusCode = 500;
            responseHttps.Error = $"Erro ao recuperar informações {e.Message}";
        }
        return responseHttps;

    }
}
