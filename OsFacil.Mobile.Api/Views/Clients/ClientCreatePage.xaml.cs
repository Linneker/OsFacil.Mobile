using OsFacil.Mobile.Api.ViewModels.Clients;

namespace OsFacil.Mobile.Api.Views.Clients;

public partial class ClientCreatePage : ContentPage
{
	public ClientCreatePage(ClientCreateViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;

    }
}