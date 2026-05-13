namespace OsFacil.Mobile.Api.ViewModels.Messages;

public sealed class MonthlyLimitExceededMessage
{
    public int? Limit { get; }
    public int? Used { get; }

    public MonthlyLimitExceededMessage(int? limit, int? used)
    {
        Limit = limit;
        Used = used;
    }
}
