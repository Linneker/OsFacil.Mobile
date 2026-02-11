using OsFacil.Mobile.Api.ViewModels.Workorders;

namespace OsFacil.Mobile.Api.Views.Workorders;

public partial class WorkorderPage : ContentPage
{
    private readonly WorkorderViewModel _vm;

    public WorkorderPage(WorkorderViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (!_vm.HasLoaded || _vm.NeedsReload || _vm.Items.Count == 0)
            _vm.ReloadCommand.Execute(null);
    }
}
