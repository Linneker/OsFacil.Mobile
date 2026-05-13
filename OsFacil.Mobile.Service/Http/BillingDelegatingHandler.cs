using System.Net;
using System.Text.Json;

namespace OsFacil.Mobile.Service.Http;

public sealed class BillingDelegatingHandler : DelegatingHandler
{
    private const string SubscriptionExpired = "SUBSCRIPTION_EXPIRED";
    private const string SubscriptionNotFound = "SUBSCRIPTION_NOT_FOUND";
    private const string MonthlyLimitExceeded = "MONTHLY_LIMIT_EXCEEDED";

    private readonly IBillingEventBus _bus;

    public BillingDelegatingHandler(IBillingEventBus bus)
    {
        _bus = bus;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Forbidden)
            return response;

        // Buffer so callers can still read the body downstream.
        await response.Content.LoadIntoBufferAsync(cancellationToken);

        BillingError? parsed = null;
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            parsed = await JsonSerializer.DeserializeAsync<BillingError>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);
        }
        catch (JsonException) { /* not a billing error body */ }
        catch (NotSupportedException) { /* unexpected content type */ }

        if (parsed is null || string.IsNullOrEmpty(parsed.Error))
            return response;

        switch (parsed.Error)
        {
            case SubscriptionExpired:
            case SubscriptionNotFound:
                _bus.RaiseExpired(parsed.Error, parsed.ValidUntil);
                break;
            case MonthlyLimitExceeded:
                _bus.RaiseMonthlyLimitExceeded(parsed.Limit, parsed.Used);
                break;
        }

        return response;
    }

    private sealed class BillingError
    {
        public string? Error { get; set; }
        public DateTime? ValidUntil { get; set; }
        public int? Limit { get; set; }
        public int? Used { get; set; }
    }
}
