using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using OsFacil.Mobile.Api.Models;
using OsFacil.Mobile.Api.Services.Billing;
using OsFacil.Mobile.Api.Services.Navigation;
using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Api.ViewModels.Clients;
using OsFacil.Mobile.Api.Views;
using OsFacil.Mobile.Service.Https.Billing;
using OsFacil.Mobile.Service.Https.Login;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace OsFacil.Mobile.Api.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private LoginModel login;

    private ILoginHttp _loginHttp;
    private readonly IToastService _toast;
    private readonly ClientViewModel _clientViewModel;
    private readonly IAuthSession _session;
    private readonly IFlyoutNavigationService _nav;
    private readonly IRootNavigator _root;
    private readonly IServiceProvider _sp;
    private readonly IBillingHttp _billingHttp;
    private readonly IBillingCacheService _billingCache;

    public ObservableCollection<LoginModel> Logins { get; set; }

    public LoginViewModel(ILoginHttp loginHttp, IToastService toast, ClientViewModel clientViewModel, IAuthSession session,
        IFlyoutNavigationService nav, IRootNavigator root, IServiceProvider sp,
        IBillingHttp billingHttp, IBillingCacheService billingCache)
    {
        _sp = sp;
        _nav = nav;
        Login = new LoginModel();
       //Login = new LoginModel()
       //{
       //    Password = "SenhaForte@123",
       //    Email = "linneker.blytner@gmail.com",
       //    Slug = "linneker-blytner-braga"
       //};
        _toast = toast;
        Logins = new ObservableCollection<LoginModel>();
        _loginHttp = loginHttp;
        _clientViewModel = clientViewModel;
        _session = session;
        _root = root;
        _billingHttp = billingHttp;
        _billingCache = billingCache;
    }

    [RelayCommand]
    private async Task Logging()
    {
        Login.Error = "";
        Logins.Add(Login);
        var response = await _loginHttp.LoginAsync(new LoginHttpRequest(Login.Slug, Login.Email, Login.Password));
        if (response.IsSuccessStatusCode)
        {
            await _toast.ShowAsync("Login realizado com sucesso!");
            await _session.SetTokenAsync(response.Data.AccessToken);

            // Verificar billing/assinatura — sempre busca fresco no login
            try
            {
                var billing = await _billingHttp.GetStatusAsync(response.Data.AccessToken);
                if (billing.IsSuccessStatusCode)
                {
                    await _billingCache.CacheAsync(billing.Data);
                    if (billing.Data.Expired)
                    {
                        _root.ShowSubscription();
                        return;
                    }
                }
            }
            catch
            {
                // Degradação graciosa: se falhar, segue pro app
            }

            _root.ShowMain();
        }
        else
        {
            Login.Error = response.Error;
            await _toast.ShowAsync(response.Error);
        }
    }

    [RelayCommand]
    private Task LogoutAsync()
    {
        _root.Logout(); // ✅ volta pro login só por aqui
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        var registerPage = _sp.GetRequiredService<RegisterPage>();
        if (Application.Current?.Windows.FirstOrDefault()?.Page is NavigationPage nav)
            await nav.PushAsync(registerPage);
    }
}
