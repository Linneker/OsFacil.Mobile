using OsFacil.Mobile.Service.Util;

namespace OsFacil.Mobile.Service.Https.Clients.Response;

public class DeleteHttpResponse(string message) : IResponseHttp, IDisposable
{
    public string Message { get; private set; } = message;
    public void Dispose()
    {
        Message = string.Empty;
    }
}