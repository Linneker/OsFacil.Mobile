using OsFacil.Mobile.Api.Models.Clients;
using OsFacil.Mobile.Api.Services.Navigation;
using OsFacil.Mobile.Api.ViewModels.Clients;

namespace OsFacil.Mobile.Api.Views.Clients;

public partial class ClientPage : ContentPage
{
    private bool _loadedOnce;
    private ClientViewModel _vm;
    public ClientPage(ClientViewModel vm, IFlyoutNavigationService nav)
	{
        InitializeComponent();
        _vm = vm;
        // carrega a primeira página
        //_ = vm.ReloadCommand.ExecuteAsync(null);
        BindingContext = vm;

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "",
            Order = ToolbarItemOrder.Primary,
            Command = new Command(nav.OpenMenu)
        });

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();


        if (BindingContext is ClientViewModel vm)
        {
            if ((!vm.HasLoaded || vm.NeedsReload || vm.Items.Count == 0) && !_loadedOnce)
            {
                _loadedOnce = true;
                vm.NeedsReload = false;
                _ = vm.ReloadCommand.ExecuteAsync(null);
            }
            else if(vm.NeedsReload)
            {
                vm.NeedsReload = false;
                _ = vm.ReloadCommand.ExecuteAsync(null);
            }
            else if (_loadedOnce) return;

        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (BindingContext is not ClientViewModel vm) return;

        var button = (ImageButton)sender;

        if (button.BindingContext is ClientModel item)
            await vm.DeleteCommand.ExecuteAsync(item); // CommunityToolkit
    }

}