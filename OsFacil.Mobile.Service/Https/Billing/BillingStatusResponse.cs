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

    [JsonPropertyName("monthlyLimit")]
    public int MonthlyLimit { get; set; }

    [JsonPropertyName("usedThisMonth")]
    public int UsedThisMonth { get; set; }
}
