using OsFacil.Mobile.Api.ViewModels.Billing;

namespace OsFacil.Mobile.Api.Views.Billing;

public partial class PaymentWebViewPage : ContentPage
{
    public PaymentWebViewPage()
    {
        InitializeComponent();
    }

    private async void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
    {
        if (BindingContext is PaymentWebViewViewModel vm)
        {
            await vm.HandleNavigationAsync(e.Url);
        }
    }
}
