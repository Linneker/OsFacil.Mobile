using OsFacil.Mobile.Service.Https.Billing;

namespace OsFacil.Mobile.Api.Services.Billing;

public interface IBillingCacheService
{
    Task<BillingStatusResponse?> GetCachedAsync();
    Task CacheAsync(BillingStatusResponse data);
    bool NeedsRefresh();
    void Clear();
}
