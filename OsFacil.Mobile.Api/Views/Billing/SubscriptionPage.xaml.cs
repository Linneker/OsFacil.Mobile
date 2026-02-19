using OsFacil.Mobile.Api.ViewModels.Billing;

namespace OsFacil.Mobile.Api.Views.Billing;

public partial class SubscriptionPage : ContentPage
{
    private readonly SubscriptionViewModel _vm;

    public SubscriptionPage(SubscriptionViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadCommand.Execute(null);
    }
}
