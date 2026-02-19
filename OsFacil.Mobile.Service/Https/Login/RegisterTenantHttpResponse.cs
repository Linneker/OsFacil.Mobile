using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Login;

public class RegisterTenantHttpResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public object? Data { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
