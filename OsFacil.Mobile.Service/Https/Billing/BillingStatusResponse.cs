using OsFacil.Mobile.Service.Util;
using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Billing;

public class BillingStatusResponse : IResponseHttp
{
    [JsonPropertyName("plan")]
    public string Plan { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("expired")]
    public bool Expired { get; set; }

    [JsonPropertyName("validUntil")]
    public DateTime? ValidUntil { get; set; }

    [JsonPropertyName("monthlyWorkOrderLimit")]
    public int MonthlyWorkOrderLimit { get; set; }

    [JsonPropertyName("monthlyWorkOrderRemaining")]
    public int MonthlyWorkOrderRemaining { get; set; }


    [JsonPropertyName("monthlyWorkOrderUsed")]
    public int UsedThisMonth { get; set; }
}
