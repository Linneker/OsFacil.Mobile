namespace OsFacil.Mobile.Api.ViewModels.Messages;

public sealed class BillingExpiredMessage
{
    public string ErrorCode { get; }
    public DateTime? ValidUntil { get; }

    public BillingExpiredMessage(string errorCode, DateTime? validUntil)
    {
        ErrorCode = errorCode;
        ValidUntil = validUntil;
    }
}
