using OsFacil.Mobile.Api.Services.Billing;

namespace OsFacil.Mobile.Api.Services.Session;

public sealed class AuthSession : IAuthSession
{
    private const string TokenKey = "auth_token";

    private readonly IBillingCacheService _billingCache;

    public AuthSession(IBillingCacheService billingCache)
    {
        _billingCache = billingCache;
    }

    public string? AccessToken { get; private set; }

    public async Task LoadAsync()
    {
        AccessToken = await SecureStorage.GetAsync(TokenKey);
    }

    public async Task SetTokenAsync(string token)
    {
        AccessToken = token;
        await SecureStorage.SetAsync(TokenKey, token);
    }

    public Task ClearAsync()
    {
        AccessToken = null;
        SecureStorage.Remove(TokenKey);
        _billingCache.Clear();
        return Task.CompletedTask;
    }
}
