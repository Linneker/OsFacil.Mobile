using OsFacil.Mobile.Service.Util;

namespace OsFacil.Mobile.Service.Https.Billing;

public interface IBillingHttp
{
    Task<ResponseHttps<BillingStatusResponse>> GetStatusAsync(string token, CancellationToken ct = default);
    Task<ResponseHttps<RenewResponse>> RenewAsync(string token, RenewRequest request, CancellationToken ct = default);
    Task<ResponseHttps<BillingInvoiceResponse>> GetInvoiceAsync(string token, Guid invoiceId, CancellationToken ct = default);
}
