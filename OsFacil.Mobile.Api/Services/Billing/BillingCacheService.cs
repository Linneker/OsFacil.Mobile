using OsFacil.Mobile.Service.Https.Billing;
using System.Text.Json;

namespace OsFacil.Mobile.Api.Services.Billing;

public class BillingCacheService : IBillingCacheService
{
    private const string DataKey = "billing_data";
    private const string DateKey = "billing_date";

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
        var dateStr = Preferences.Get(DateKey, string.Empty);
        if (string.IsNullOrEmpty(dateStr))
            return true;

        if (!DateTime.TryParse(dateStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var lastCheck))
            return true;

        return (DateTime.UtcNow - lastCheck).TotalHours >= 24;
    }

    public void Clear()
    {
        Preferences.Remove(DataKey);
        Preferences.Remove(DateKey);
    }
}
