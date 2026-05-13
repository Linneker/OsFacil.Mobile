using OsFacil.Mobile.Service.Util;
using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Billing;

public class BillingInvoiceResponse : IResponseHttp
{
    [JsonPropertyName("invoiceId")]
    public Guid InvoiceId { get; set; }

    [JsonPropertyName("plan")]
    public string Plan { get; set; } = string.Empty;

    [JsonPropertyName("months")]
    public int Months { get; set; }

    [JsonPropertyName("amountCents")]
    public int AmountCents { get; set; }

    [JsonPropertyName("workOrderLimit")]
    public int? WorkOrderLimit { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("preferenceId")]
    public string? PreferenceId { get; set; }

    [JsonPropertyName("paymentId")]
    public long? PaymentId { get; set; }
}
