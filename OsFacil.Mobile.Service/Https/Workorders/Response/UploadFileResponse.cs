using OsFacil.Mobile.Service.Util;
using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Workorders.Response;

public class UploadFileResponse : IResponseHttp
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("file")]
    public string File { get; set; }
}
