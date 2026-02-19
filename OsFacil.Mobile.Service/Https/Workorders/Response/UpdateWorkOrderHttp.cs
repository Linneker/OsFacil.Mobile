using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Workorders.Response;

public class UpdateWorkOrderHttp 
{
    [JsonPropertyName("clientId")]
    public string ClientId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}
