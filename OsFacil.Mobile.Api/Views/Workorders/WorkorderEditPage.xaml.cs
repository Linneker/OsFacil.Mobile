using OsFacil.Mobile.Api.ViewModels.Workorders;

namespace OsFacil.Mobile.Api.Views.Workorders;

public partial class WorkorderEditPage : ContentPage
{
    private readonly WorkorderEditViewModel _vm;

    public WorkorderEditPage(WorkorderEditViewModel vm)
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

    private void OnImageTapped(object sender, TappedEventArgs e)
    {
        if (sender is View view && view.BindingContext is ImageSource imgSource)
            _vm.ExpandImageCommand.Execute(imgSource);
    }
}
