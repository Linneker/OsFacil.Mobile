using OsFacil.Mobile.Api.ViewModels;

namespace OsFacil.Mobile.Api.Views;

public partial class MenuPage : ContentPage
{
	public MenuPage(MenuViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    private async void SendPageMenu(object sender, EventArgs e)
    {
        if (BindingContext is not MenuViewModel vm) return;

        var button = (Button)sender;

        if (button.BindingContext is MenuItemVm item)
            await vm.SelectCommand.ExecuteAsync(item); // CommunityToolkit

    }
}