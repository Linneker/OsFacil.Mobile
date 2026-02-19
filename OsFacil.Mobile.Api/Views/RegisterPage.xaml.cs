using OsFacil.Mobile.Api.ViewModels;

namespace OsFacil.Mobile.Api.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
