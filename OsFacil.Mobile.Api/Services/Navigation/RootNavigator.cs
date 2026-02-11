using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Api.Views;

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
        await _session.ClearAsync(); // ou _session.Clear()
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