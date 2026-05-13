using OsFacil.Mobile.Service.Https.Billing;
using System.Globalization;
using System.Text.Json;

namespace OsFacil.Mobile.Api.Services.Billing;

public class BillingCacheService : IBillingCacheService
{
    private const string DataKey = "billing_data";
    private const string DateKey = "billing_date";
    private static readonly TimeSpan Ttl = TimeSpan.FromHours(1);

    public async Task<BillingStatusResponse?> GetCachedAsync()
    {
        var json = Preferences.Get(DataKey, string.Empty);
        if (string.IsNullOrEmpty(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<BillingStatusResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    public Task CacheAsync(BillingStatusResponse data)
    {
        var json = JsonSerializer.Serialize(data);
        Preferences.Set(DataKey, json);
        Preferences.Set(DateKey, DateTime.UtcNow.ToString("o"));
        return Task.CompletedTask;
    }

    public bool NeedsRefresh()
    {
        var last = LastCheckUtc;
        if (last is null) return true;
        return DateTime.UtcNow - last.Value >= Ttl;
    }

    public DateTime? LastCheckUtc
    {
        get
        {
            var dateStr = Preferences.Get(DateKey, string.Empty);
            if (string.IsNullOrEmpty(dateStr))
                return null;
            if (!DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
                return null;
            return parsed;
        }
    }

    public void Clear()
    {
        Preferences.Remove(DataKey);
        Preferences.Remove(DateKey);
    }
}
