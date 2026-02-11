using OsFacil.Mobile.Api.ViewModels;

namespace OsFacil.Mobile.Api.Views.Dashboard;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _vm;

    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (!_vm.HasLoaded)
            _vm.ReloadCommand.Execute(null);
    }
}