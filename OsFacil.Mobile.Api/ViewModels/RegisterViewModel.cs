using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OsFacil.Mobile.Api.Models;
using OsFacil.Mobile.Api.Services;
using OsFacil.Mobile.Service.Https.Login;

namespace OsFacil.Mobile.Api.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly ILoginHttp _loginHttp;
    private readonly IToastService _toast;

    public RegisterViewModel(ILoginHttp loginHttp, IToastService toast)
    {
        _loginHttp = loginHttp;
        _toast = toast;
    }

    [ObservableProperty]
    private RegisterModel register = new();

    [ObservableProperty]
    private bool isBusy;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy) return;

        Register.Error = "";

        if (string.IsNullOrWhiteSpace(Register.CompanyName) ||
            string.IsNullOrWhiteSpace(Register.CompanySlug) ||
            string.IsNullOrWhiteSpace(Register.AdminName) ||
            string.IsNullOrWhiteSpace(Register.AdminEmail) ||
            string.IsNullOrWhiteSpace(Register.AdminPassword))
        {
            Register.Error = "Preencha todos os campos.";
            await _toast.ShowAsync("Preencha todos os campos.");
            return;
        }

        try
        {
            IsBusy = true;

            var request = new RegisterTenantHttpRequest(
                Register.CompanyName!,
                Register.CompanySlug!,
                Register.AdminName!,
                Register.AdminEmail!,
                Register.AdminPassword!);

            var response = await _loginHttp.RegisterTenantAsync(request);

            if (response.Success)
            {
                await _toast.ShowAsync("Cadastro realizado com sucesso!");
                await GoBackAsync();
            }
            else
            {
                Register.Error = response.Error ?? "Erro ao cadastrar.";
                await _toast.ShowAsync(Register.Error);
            }
        }
        catch
        {
            Register.Error = "Erro ao realizar cadastro.";
            await _toast.ShowAsync(Register.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        if (Application.Current?.Windows.FirstOrDefault()?.Page is NavigationPage nav)
            await nav.PopAsync();
    }
}
