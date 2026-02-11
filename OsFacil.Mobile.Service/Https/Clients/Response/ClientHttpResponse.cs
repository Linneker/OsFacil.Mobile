using OsFacil.Mobile.Service.Util;
using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Clients.Response;

public class ClientHttpResponse : IResponseHttp, IDisposable
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]

    public string Name { get; set; }
    [JsonPropertyName("phone")]

    public string Phone { get; set; }
    [JsonPropertyName("email")]

    public string Email { get; set; }

    public void Dispose()
    {
        Id = string.Empty;
        Name = string.Empty;
        Phone = string.Empty;
        Email = string.Empty;
    }
}