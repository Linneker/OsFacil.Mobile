using OsFacil.Mobile.Api.ViewModels;

namespace OsFacil.Mobile.Api.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}