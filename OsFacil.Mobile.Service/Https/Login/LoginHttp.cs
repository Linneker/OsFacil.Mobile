using OsFacil.Mobile.Service.Util;
using System.Net.Http.Json;
using System.Text.Json;

namespace OsFacil.Mobile.Service.Https.Login;

public class LoginHttp : ILoginHttp
{
    private readonly HttpClient _httpClient;

    public LoginHttp(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResponseHttps<LoginHttpResponse>> LoginAsync(LoginHttpRequest request, CancellationToken ct = default)
    {
        ResponseHttps<LoginHttpResponse> responseHttps = new ResponseHttps<LoginHttpResponse>();

        try
        {
            using var response = await _httpClient.PostAsJsonAsync("/api/auth/login", value: request,
                cancellationToken: ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);

                // Aqui você decide: lançar, retornar erro no response, etc.
                // Vou lançar com detalhes pra você enxergar o problema no debug.
                responseHttps.IsSuccessStatusCode = false;
                responseHttps.StatusCode = (int)response.StatusCode;
                responseHttps.Error = string.IsNullOrEmpty(errorBody) ?  "Slug/Email/Senha Invalid!": errorBody;
                return responseHttps;
            }


            var result = await response.Content.ReadFromJsonAsync<LoginHttpResponse>(
                options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken: ct);
            responseHttps.Data = result ?? throw new InvalidOperationException("Resposta de login veio vazia (null)."); ;
            responseHttps.IsSuccessStatusCode = response.IsSuccessStatusCode;
            responseHttps.StatusCode = (int)response.StatusCode;
            return responseHttps;
        }
        catch (Exception e)
        {
            responseHttps.IsSuccessStatusCode = false;
            responseHttps.StatusCode = 500;
            responseHttps.Error = "Ocorreu um erro ao tentar fazer login: " + e.Message;
            return responseHttps;
        }
    }
}
