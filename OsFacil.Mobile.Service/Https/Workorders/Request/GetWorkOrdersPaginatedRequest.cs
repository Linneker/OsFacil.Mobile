using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Workorders.Request;

public class GetWorkOrdersPaginatedRequest
{
    [JsonPropertyName("pageNumber")]
    public int PageNumber{ get; set; }
    [JsonPropertyName("pageSize")]
    public int PageSize{ get; set; }
    [JsonPropertyName("sortBy")]
    public string? SortBy { get; set; } = null;
    [JsonPropertyName("sortDirection")]
    public int SortDirection { get; set; } = 0;

    [JsonPropertyName("clientNameOrSlug")] 
    public string? ClientNameOrSlug { get; set; }
    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }
    [JsonPropertyName("startCreatedAt")] 
    public DateTime? StartCreatedAt { get; set; }
    [JsonPropertyName("finishedCreatedAt")]
    public DateTime? FinishedCreatedAt { get; set; }
    [JsonPropertyName("startFinishedAt")] 
    public DateTime? StartFinishedAt { get; set; }
    [JsonPropertyName("finesedFinishedAt")] 
    public DateTime? FinesedFinishedAt { get; set; }

}
