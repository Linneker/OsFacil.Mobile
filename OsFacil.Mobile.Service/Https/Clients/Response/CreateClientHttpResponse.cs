using OsFacil.Mobile.Service.Util;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace OsFacil.Mobile.Service.Https.Clients.Response;

public class CreateClientHttpResponse : IResponseHttp, IDisposable
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    public void Dispose()
    {
        Id = null;
    }

}
