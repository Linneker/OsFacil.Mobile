using OsFacil.Mobile.Service.Util;
using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Workorders.Response;

public class GetWorkorderHttpResponse : IResponseHttp
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("clientId")]
    public Guid ClientId { get; set; }
    [JsonPropertyName("clientName")]
    public string ClientName { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("status")]
    public int Status { get; set; }
    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("finishedAt")]
    public DateTime? FinishedAt { get; set; }

}
