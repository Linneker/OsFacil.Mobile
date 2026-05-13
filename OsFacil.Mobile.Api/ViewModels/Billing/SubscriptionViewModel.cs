using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OsFacil.Mobile.Api.Models.Billing;
using OsFacil.Mobile.Api.Services.Billing;
using OsFacil.Mobile.Api.Services.Navigation;
using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Api.Views.Billing;
using OsFacil.Mobile.Service.Https.Billing;

namespace OsFacil.Mobile.Api.ViewModels.Billing;

public partial class SubscriptionViewModel : ObservableObject
{
    [ObservableProperty]
    private SubscriptionModel subscription;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isRefreshing;

    private readonly IBillingHttp _billingHttp;
    private readonly IBillingCacheService _cache;
    private readonly IAuthSession _session;
    private readonly IRootNavigator _root;
    private readonly IToastService _toast;
    private readonly IServiceProvider _sp;

    public List<string> PlanOptions { get; } = ["Basic", "Flex", "Pro"];

    public SubscriptionViewModel(
        IBillingHttp billingHttp,
        IBillingCacheService cache,
        IAuthSession session,
        IRootNavigator root,
        IToastService toast,
        IServiceProvider sp,
        SubscriptionModel subscription)
    {
        _billingHttp = billingHttp;
        _cache = cache;
        _session = session;
        _root = root;
        _toast = toast;
        _sp = sp;
        Subscription = subscription;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var cached = await _cache.GetCachedAsync();
            if (cached != null && !_cache.NeedsRefresh())
            {
                MapToModel(cached);
                return;
            }

            var token = _session.AccessToken;
            if (string.IsNullOrEmpty(token)) return;

            var response = await _billingHttp.GetStatusAsync(token);
            if (response.IsSuccessStatusCode)
            {
                await _cache.CacheAsync(response.Data);
                MapToModel(response.Data);
            }
            else
            {
                await _toast.ShowAsync(response.Error ?? "Erro ao carregar assinatura");
            }
        }
        catch (Exception e)
        {
            await _toast.ShowAsync($"Erro: {e.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task PayAsync()
    {
        var token = _session.AccessToken;
        if (string.IsNullOrEmpty(token)) return;

        IsLoading = true;
        try
        {
            var request = new RenewRequest
            {
                Plan = Subscription.SelectedPlanName.ToLowerInvariant(),
                Months = Subscription.Months,
                WorkOrderLimit = Subscription.GetWorkOrderLimit()
            };

            var response = await _billingHttp.RenewAsync(token, request);
            if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(response.Data.CheckoutUrl))
            {
                var webViewPage = _sp.GetRequiredService<PaymentWebViewPage>();
                var webViewVm = _sp.GetRequiredService<PaymentWebViewViewModel>();
                webViewVm.PaymentUrl = response.Data.CheckoutUrl;
                webViewVm.InvoiceId = Guid.TryParse(response.Data.InvoiceId, out var invId) ? invId : null;
                webViewPage.BindingContext = webViewVm;

                if (Application.Current?.Windows.FirstOrDefault()?.Page is NavigationPage nav)
                    await nav.PushAsync(webViewPage);
                else if (Application.Current?.Windows.FirstOrDefault()?.Page is FlyoutPage flyout
                         && flyout.Detail is NavigationPage detailNav)
                    await detailNav.PushAsync(webViewPage);
            }
            else
            {
                await _toast.ShowAsync(response.Error ?? "Erro ao gerar pagamento");
            }
        }
        catch (Exception e)
        {
            await _toast.ShowAsync($"Erro: {e.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        var token = _session.AccessToken;
        if (string.IsNullOrEmpty(token))
        {
            IsRefreshing = false;
            return;
        }

        try
        {
            var response = await _billingHttp.GetStatusAsync(token);
            if (response.IsSuccessStatusCode && response.Data is not null)
            {
                await _cache.CacheAsync(response.Data);
                MapToModel(response.Data);
            }
            else
            {
                await _toast.ShowAsync(response.Error ?? "Erro ao atualizar assinatura");
            }
        }
        catch (Exception e)
        {
            await _toast.ShowAsync($"Erro: {e.Message}");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private void GoToMain()
    {
        _root.ShowMain();
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _root.Logout();
    }

    private void MapToModel(BillingStatusResponse data)
    {
        Subscription.Plan = data.Plan ?? string.Empty;
        Subscription.Status = data.Status ?? string.Empty;
        Subscription.Expired = data.Expired;
        Subscription.ValidUntil = data.ValidUntil;
        Subscription.MonthlyLimit = data.MonthlyWorkOrderLimit;
        Subscription.UsedThisMonth = data.UsedThisMonth;
        Subscription.RecalculatePrice();
    }
}
