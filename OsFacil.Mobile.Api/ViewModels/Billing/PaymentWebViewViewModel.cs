using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OsFacil.Mobile.Api.Services;
using OsFacil.Mobile.Api.Services.Billing;
using OsFacil.Mobile.Api.Services.Navigation;
using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Service.Https.Billing;

namespace OsFacil.Mobile.Api.ViewModels.Billing;

public partial class PaymentWebViewViewModel : ObservableObject
{
    private const int MaxPollAttempts = 20;
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(3);

    [ObservableProperty]
    private string paymentUrl = string.Empty;

    [ObservableProperty]
    private bool isNavigating = true;

    [ObservableProperty]
    private bool isConfirmingPayment;

    public Guid? InvoiceId { get; set; }

    private CancellationTokenSource? _pollingCts;

    private readonly IRootNavigator _root;
    private readonly IToastService _toast;
    private readonly IBillingCacheService _cache;
    private readonly IBillingHttp _billingHttp;
    private readonly IAuthSession _session;

    public PaymentWebViewViewModel(
        IRootNavigator root,
        IToastService toast,
        IBillingCacheService cache,
        IBillingHttp billingHttp,
        IAuthSession session)
    {
        _root = root;
        _toast = toast;
        _cache = cache;
        _billingHttp = billingHttp;
        _session = session;
    }

    public async Task HandleNavigationAsync(string url)
    {
        if (string.IsNullOrEmpty(url) || IsConfirmingPayment) return;

        var lower = url.ToLowerInvariant();

        if (lower.Contains("/success") || lower.Contains("/approved"))
        {
            await ConfirmPaymentAsync();
        }
        else if (lower.Contains("/failure") || lower.Contains("/cancelled"))
        {
            await _toast.ShowAsync("Pagamento cancelado ou falhou.");
            await PopAsync();
        }
    }

    private async Task ConfirmPaymentAsync()
    {
        var token = _session.AccessToken;
        if (string.IsNullOrEmpty(token))
        {
            await PopAsync();
            return;
        }

        IsConfirmingPayment = true;
        _pollingCts = new CancellationTokenSource();
        var ct = _pollingCts.Token;

        try
        {
            // First check: maybe webhook already arrived.
            var status = await _billingHttp.GetStatusAsync(token, ct);
            if (status.IsSuccessStatusCode && status.Data is not null && !status.Data.Expired)
            {
                await CompleteSuccessAsync(status.Data);
                return;
            }

            // Poll the invoice status.
            if (InvoiceId is null)
            {
                // Fallback: poll /me only.
                if (await PollStatusAsync(token, ct))
                    return;
            }
            else
            {
                if (await PollInvoiceAsync(token, InvoiceId.Value, ct))
                    return;
            }

            await _toast.ShowAsync("Pagamento em processamento — verifique em instantes.");
            await PopAsync();
        }
        catch (OperationCanceledException)
        {
            await PopAsync();
        }
        finally
        {
            IsConfirmingPayment = false;
            _pollingCts?.Dispose();
            _pollingCts = null;
        }
    }

    private async Task<bool> PollInvoiceAsync(string token, Guid invoiceId, CancellationToken ct)
    {
        for (var i = 0; i < MaxPollAttempts; i++)
        {
            await Task.Delay(PollInterval, ct);
            var inv = await _billingHttp.GetInvoiceAsync(token, invoiceId, ct);
            if (inv.IsSuccessStatusCode && inv.Data is not null &&
                string.Equals(inv.Data.Status, "Paid", StringComparison.OrdinalIgnoreCase))
            {
                var status = await _billingHttp.GetStatusAsync(token, ct);
                if (status.IsSuccessStatusCode && status.Data is not null)
                    await CompleteSuccessAsync(status.Data);
                else
                    await CompleteSuccessAsync(null);
                return true;
            }
        }
        return false;
    }

    private async Task<bool> PollStatusAsync(string token, CancellationToken ct)
    {
        for (var i = 0; i < MaxPollAttempts; i++)
        {
            await Task.Delay(PollInterval, ct);
            var status = await _billingHttp.GetStatusAsync(token, ct);
            if (status.IsSuccessStatusCode && status.Data is not null && !status.Data.Expired)
            {
                await CompleteSuccessAsync(status.Data);
                return true;
            }
        }
        return false;
    }

    private async Task CompleteSuccessAsync(BillingStatusResponse? freshStatus)
    {
        _cache.Clear();
        if (freshStatus is not null)
            await _cache.CacheAsync(freshStatus);
        await _toast.ShowAsync("Pagamento confirmado!");
        await PopAsync();
        _root.ShowMain();
    }

    [RelayCommand]
    private async Task CancelPollingAsync()
    {
        _pollingCts?.Cancel();
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CloseAsync()
    {
        _pollingCts?.Cancel();
        await PopAsync();
    }

    private async Task PopAsync()
    {
        var page = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (page is NavigationPage nav)
            await nav.PopAsync();
        else if (page is FlyoutPage flyout && flyout.Detail is NavigationPage detailNav)
            await detailNav.PopAsync();
    }
}
