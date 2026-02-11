using OsFacil.Mobile.Api.ViewModels.Workorders;

namespace OsFacil.Mobile.Api.Views.Workorders;

public partial class WorkorderCreatePage : ContentPage
{
    private readonly WorkorderCreateViewModel _vm;

    public WorkorderCreatePage(WorkorderCreateViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadClientsCommand.Execute(null);
    }
}
