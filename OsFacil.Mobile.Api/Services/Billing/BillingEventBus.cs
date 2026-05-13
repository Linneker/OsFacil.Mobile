using CommunityToolkit.Mvvm.Messaging;
using OsFacil.Mobile.Api.ViewModels.Messages;
using OsFacil.Mobile.Service.Http;

namespace OsFacil.Mobile.Api.Services.Billing;

public sealed class BillingEventBus : IBillingEventBus
{
    public void RaiseExpired(string errorCode, DateTime? validUntil)
        => WeakReferenceMessenger.Default.Send(new BillingExpiredMessage(errorCode, validUntil));

    public void RaiseMonthlyLimitExceeded(int? limit, int? used)
        => WeakReferenceMessenger.Default.Send(new MonthlyLimitExceededMessage(limit, used));
}
