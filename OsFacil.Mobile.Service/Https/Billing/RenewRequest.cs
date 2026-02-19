using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Billing;

public class RenewRequest
{
    [JsonPropertyName("plan")]
    public string Plan { get; set; } = string.Empty;

    [JsonPropertyName("months")]
    public int Months { get; set; }

    [JsonPropertyName("workOrderLimit")]
    public int WorkOrderLimit { get; set; }
}
