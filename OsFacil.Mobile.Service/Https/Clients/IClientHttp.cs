using OsFacil.Mobile.Service.Https.Clients.Request;
using OsFacil.Mobile.Service.Https.Clients.Response;
using OsFacil.Mobile.Service.Util;

namespace OsFacil.Mobile.Service.Https.Clients;

public interface IClientHttp
{
    public Task<ResponseHttps<DeleteHttpResponse>> DeleteClientsAsync(string token, string id,string name, CancellationToken ct = default);

    public Task<ResponsesHttps<ClientHttpResponse>> GetClientsAsync(string token, string search, int page, int pageSize, CancellationToken ct = default);
    public Task<ResponseHttps<CreateClientHttpResponse>> CreateClientAsync(string token, CreateClientHttpRequest request, CancellationToken ct = default);
}