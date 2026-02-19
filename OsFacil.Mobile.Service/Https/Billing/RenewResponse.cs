using OsFacil.Mobile.Service.Util;
using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Billing;

public class RenewResponse : IResponseHttp
{
    [JsonPropertyName("invoiceId")]
    public string InvoiceId { get; set; } = string.Empty;

    [JsonPropertyName("plan")]
    public string Plan { get; set; } = string.Empty;

    [JsonPropertyName("months")]
    public int Months { get; set; }

    [JsonPropertyName("amountCents")]
    public int AmountCents { get; set; }
    
    [JsonPropertyName("checkoutUrl")]
    public string CheckoutUrl { get; set; } = string.Empty;

}