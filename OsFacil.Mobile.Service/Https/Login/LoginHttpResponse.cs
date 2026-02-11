using OsFacil.Mobile.Service.Util;
using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Login;

public class LoginHttpResponse : IResponseHttp
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }
    [JsonPropertyName("displayName")] 
    public string DisplayName { get; set; }

}