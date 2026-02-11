using OsFacil.Mobile.Api.Views;
using OsFacil.Mobile.Api.Views.Clients;
using OsFacil.Mobile.Api.Views.Dashboard;
using OsFacil.Mobile.Api.Views.Workorders;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Api.Services.Navigation;

public class FlyoutNavigationService : IFlyoutNavigationService
{
    private FlyoutPage? _host;
    private readonly IServiceProvider _sp;

    public FlyoutNavigationService(IServiceProvider sp)
    {
        _sp = sp;
    }

    public void SetHost(FlyoutPage host) => _host = host;

    public void OpenMenu()
    {
        if (_host != null) _host.IsPresented = true;
    }

    public void CloseMenu()
    {
        if (_host != null) _host.IsPresented = false;
    }

    private NavigationPage GetNav()
    {
        if (_host is null)
            throw new InvalidOperationException("Flyout host não foi definido. Chame SetHost(this) no MainFlyoutPage.");

        if (_host.Detail is NavigationPage nav)
            return nav;

        // fallback: se alguém setou Detail errado
        var newNav = new NavigationPage(new ContentPage { Title = "App" });
        _host.Detail = newNav;
        return newNav;
    }

    public Task NavigateToAsync(string key)
    {
        if (_host is null) return Task.CompletedTask;

        Page page = key switch
        {
            "dashboard" => _sp.GetRequiredService<DashboardPage>(),
            "clients" => _sp.GetRequiredService<ClientPage>(),
            "workorders" => _sp.GetRequiredService<WorkorderPage>(),
            //"subscriptions" => _sp.GetRequiredService<SubscriptionPage>(),
            //"profile" => _sp.GetRequiredService<ProfilePage>(),
            //_ => _sp.GetRequiredService<DashboardPage>(),
            _ => _sp.GetRequiredService<ClientPage>()
        };


        return MainThread.InvokeOnMainThreadAsync(() =>
        {
            _host.Detail = new NavigationPage(page); // reset stack
            _host.IsPresented = false;
        });

        // coloca a página dentro de um NavigationPage (pra ter Title/Toolbar)
        //_host.Detail = new NavigationPage(page);

        // fecha o menu após navegar
        //_host.IsPresented = false;

        //return Task.CompletedTask;
    }

    // ✅ NAVEGAÇÃO DENTRO DA SEÇÃO: empilha página (aí existe VOLTAR)
    public Task PushAsync(string key)
    {
        Page page = key switch
        {
            "createClient" => _sp.GetRequiredService<ClientCreatePage>(),
            "createWorkspace" => _sp.GetRequiredService<WorkorderCreatePage>(),
            _ => throw new ArgumentException($"Rota inválida: {key}")
        };

        var nav = GetNav();

        return MainThread.InvokeOnMainThreadAsync(async () =>
        {
            _host!.IsPresented = false;
            await nav.Navigation.PushAsync(page);
        });
    }

    // ✅ VOLTAR
    public Task PopAsync()
    {
        var nav = GetNav();

        return MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (nav.Navigation.NavigationStack.Count > 1)
                await nav.Navigation.PopAsync();
            // se estiver na raiz, não faz nada (ou você pode abrir o menu, ou ignorar)
        });
    }
}
