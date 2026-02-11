using OsFacil.Mobile.Service.Util;
using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Workorders.Response;

public class CreateWorkOrderResponse : IResponseHttp
{
    [JsonPropertyName("id")]
    public Guid Id{ get; set; }
}
