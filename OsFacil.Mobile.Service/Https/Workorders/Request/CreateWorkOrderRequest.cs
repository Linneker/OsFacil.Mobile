using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Workorders.Request;

public class CreateWorkOrderRequest
{
    [JsonPropertyName("clientId")]
    public Guid ClientId { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }
}
