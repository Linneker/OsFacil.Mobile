using OsFacil.Mobile.Api.Models.Workorders;
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

    private void OnOpenExecutionClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is WorkorderModel item)
            _vm.OpenExecutionCommand.Execute(item);
    }
}
