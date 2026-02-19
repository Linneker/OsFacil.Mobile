using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OsFacil.Mobile.Api.Services.Billing;
using OsFacil.Mobile.Api.Services.Navigation;

namespace OsFacil.Mobile.Api.ViewModels.Billing;

public partial class PaymentWebViewViewModel : ObservableObject
{
    [ObservableProperty]
    private string paymentUrl = string.Empty;

    [ObservableProperty]
    private bool isNavigating = true;

    private readonly IRootNavigator _root;
    private readonly IToastService _toast;
    private readonly IBillingCacheService _cache;

    public PaymentWebViewViewModel(IRootNavigator root, IToastService toast, IBillingCacheService cache)
    {
        _root = root;
        _toast = toast;
        _cache = cache;
    }

    public async Task HandleNavigationAsync(string url)
    {
        if (string.IsNullOrEmpty(url)) return;

        var lower = url.ToLowerInvariant();

        if (lower.Contains("/success") || lower.Contains("/approved"))
        {
            _cache.Clear(); // limpa cache para forçar refresh
            await _toast.ShowAsync("Pagamento realizado com sucesso!");
            await PopAndShowMain();
        }
        else if (lower.Contains("/failure") || lower.Contains("/cancelled"))
        {
            await _toast.ShowAsync("Pagamento cancelado ou falhou.");
            await PopAsync();
        }
    }

    [RelayCommand]
    private async Task CloseAsync()
    {
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

    private async Task PopAndShowMain()
    {
        await PopAsync();
        _root.ShowMain();
    }
}
