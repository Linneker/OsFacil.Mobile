namespace OsFacil.Mobile.Service.Http;

public interface IBillingEventBus
{
    void RaiseExpired(string errorCode, DateTime? validUntil);
    void RaiseMonthlyLimitExceeded(int? limit, int? used);
}
