using CommunityToolkit.Mvvm.Messaging;
using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Api.ViewModels.Messages;
using OsFacil.Mobile.Api.Views;
using OsFacil.Mobile.Api.Views.Billing;

namespace OsFacil.Mobile.Api.Services.Navigation;

public sealed class RootNavigator : IRootNavigator
{
    private readonly IServiceProvider _sp;
    private readonly IAuthSession _session; // seu serviço de sessão/token
    public RootNavigator(IServiceProvider sp, IAuthSession session)
    {
        _sp = sp;
        _session = session;
    }

    public async Task Logout()
    {
        await _session.ClearAsync();
        WeakReferenceMessenger.Default.Send(new SessionClearedMessage());
        ShowLogin();
    }

    public void ShowLogin()
    {
        var login = _sp.GetRequiredService<LoginPage>();
        SetRoot(new NavigationPage(login));
    }

    public void ShowMain()
    {
        var main = _sp.GetRequiredService<FlyoutApp>();
        SetRoot(main);
    }

    public void ShowSubscription()
    {
        var page = _sp.GetRequiredService<SubscriptionPage>();
        SetRoot(new NavigationPage(page));
    }

    private static void SetRoot(Page page)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var app = Application.Current;
            var window = app?.Windows.FirstOrDefault();

            if (window != null)
                window.Page = page;
            else
                app!.MainPage = page; // fallback raro
        });
    }

}