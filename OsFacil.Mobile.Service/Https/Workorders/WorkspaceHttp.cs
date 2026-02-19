using OsFacil.Mobile.Service.Https.Workorders.Request;
using OsFacil.Mobile.Service.Https.Workorders.Response;
using OsFacil.Mobile.Service.Util;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace OsFacil.Mobile.Service.Https.Workorders;

public class WorkspaceHttp : IWorkspaceHttp
{
    private readonly HttpClient _httpClient;

    public WorkspaceHttp(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResponseHttps<CreateWorkOrderResponse>> CreateWorkOrderAsync(string token, CreateWorkOrderRequest request, CancellationToken ct = default)
    {
        ResponseHttps<CreateWorkOrderResponse> responseHttps = new ResponseHttps<CreateWorkOrderResponse>();
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            using var response = await _httpClient.PostAsJsonAsync($"/api/workorders", request,
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
                var result = await response.Content.ReadFromJsonAsync<CreateWorkOrderResponse>(
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

    public async Task<ResponseHttps<GetWorkOrdersByIdResponse>> GetWorkOrdersByIdAsync(string token, Guid id, CancellationToken ct = default)
    {
        ResponseHttps<GetWorkOrdersByIdResponse> responseHttps = new ResponseHttps<GetWorkOrdersByIdResponse>();
        try
        {
            var url = $"api/workorders/{id}";

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            using var response = await _httpClient.GetAsync(url, cancellationToken: ct);

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
                var result = await response.Content.ReadFromJsonAsync<GetWorkOrdersByIdResponse>(
        options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
        cancellationToken: ct);
                responseHttps.Data = result ?? new();
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

    public async Task<ResponsesHttps<GetWorkorderHttpResponse>> GetWorkOrdersPaginatedAsync(string token, GetWorkOrdersPaginatedRequest getWorkOrdersPaginatedRequest, CancellationToken ct = default)
    {
        ResponsesHttps<GetWorkorderHttpResponse> responseHttps = new ResponsesHttps<GetWorkorderHttpResponse>();
        try
        {
            var url = "api/workorders/paginated".AddQueryString(getWorkOrdersPaginatedRequest);

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            using var response = await _httpClient.GetAsync(url,
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
                var strinJson = await response.Content.ReadAsStringAsync();
                var result = await response.Content.ReadFromJsonAsync<PagedResponse<GetWorkorderHttpResponse>>(
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

    public async Task<ResponseHttps<CreateWorkOrderResponse>> UpadateWorkOrderAsync(string token, Guid id, UpdateWorkOrderHttp request, CancellationToken ct = default)
    {
        var url = $"api/workorders/{id}";

        var responseHttps = new ResponseHttps<CreateWorkOrderResponse>();
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            using var response = await _httpClient.PutAsJsonAsync(url, request,
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
                responseHttps.Data = new CreateWorkOrderResponse() { Id = id };
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

    public async Task<ResponseHttps<UploadFileResponse>> UploadFileAsync(string token, Guid id, int kind, Stream fileStream, string fileName, string contentType, Dictionary<string, string>? fields = null, CancellationToken ct = default)
    {
        using var form = new MultipartFormDataContent();

        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        form.Add(fileContent, name: "file", fileName: fileName);

        // Campos extras (opcional)
        if (fields is not null)
        {
            foreach (var (key, value) in fields)
                form.Add(new StringContent(value), name: key);
        }
        var url = $"api/workorders/{id}/photos?kind={kind}";
        // Se seu HttpClient já tem BaseAddress, pode usar URL relativa
        var response = await _httpClient.PostAsync(url, form);
        response.EnsureSuccessStatusCode();

       var  responseHttps = new ResponseHttps<UploadFileResponse>();

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
            var result = await response.Content.ReadFromJsonAsync<UploadFileResponse>(
    options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
    cancellationToken: ct);
            if (result is null)
            {
                responseHttps.IsSuccessStatusCode = false;
                responseHttps.StatusCode = (int)response.StatusCode;
                responseHttps.Error = "Arquivo não registrado";
            }
            else
            {
                responseHttps.Data = result;
                responseHttps.IsSuccessStatusCode = true;
                responseHttps.StatusCode = (int)response.StatusCode;
            }
        }
        return responseHttps;
    }

    public async Task<Stream?> GetWorkOrderPdfAsync(string token, Guid id, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/workorders/{id}/pdf", ct);
            if (!response.IsSuccessStatusCode) return null;

            var ms = new MemoryStream();
            await response.Content.CopyToAsync(ms, ct);
            ms.Position = 0;
            return ms;
        }
        catch { return null; }
    }

    public async Task<byte[]?> DownloadPhotoAsync(string token, string url, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadAsByteArrayAsync(ct);
        }
        catch { return null; }
    }
}
