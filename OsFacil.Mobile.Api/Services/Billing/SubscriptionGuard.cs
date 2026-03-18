using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Service.Https.Billing;

namespace OsFacil.Mobile.Api.Services.Billing;

public class SubscriptionGuard : ISubscriptionGuard
{
    private readonly IBillingCacheService _cache;
    private readonly IBillingHttp _billingHttp;
    private readonly IAuthSession _session;

    public SubscriptionGuard(IBillingCacheService cache, IBillingHttp billingHttp, IAuthSession session)
    {
        _cache = cache;
        _billingHttp = billingHttp;
        _session = session;
    }

    public async Task<bool> IsExpiredAsync()
    {
        var cached = await _cache.GetCachedAsync();

        // Atualiza o cache se: não existe, está vencido (24h) ou a data de validade da assinatura já passou
        bool needsFreshCheck = cached is null
            || _cache.NeedsRefresh()
            || (cached.ValidUntil.HasValue && cached.ValidUntil.Value <= DateTime.UtcNow);

        if (needsFreshCheck && !string.IsNullOrWhiteSpace(_session.AccessToken))
        {
            var response = await _billingHttp.GetStatusAsync(_session.AccessToken);
            if (response.IsSuccessStatusCode && response.Data is not null)
            {
                await _cache.CacheAsync(response.Data);
                return response.Data.Expired;
            }
        }

        return cached?.Expired == true;
    }
}
