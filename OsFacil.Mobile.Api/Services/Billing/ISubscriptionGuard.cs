namespace OsFacil.Mobile.Api.Services.Billing;

public interface ISubscriptionGuard
{
    Task<bool> IsExpiredAsync();
}
