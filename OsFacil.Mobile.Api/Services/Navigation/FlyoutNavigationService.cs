using CommunityToolkit.Mvvm.Input;
using OsFacil.Mobile.Api.Services.Billing;
using OsFacil.Mobile.Api.Views;
using OsFacil.Mobile.Api.Views.Billing;
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
    private readonly IRootNavigator _root;
    private readonly ISubscriptionGuard _guard;

    public FlyoutNavigationService(IServiceProvider sp, IRootNavigator root, ISubscriptionGuard guard)
    {
        _sp = sp;
        _root = root;
        _guard = guard;
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

    public async Task NavigateToAsync(string key)
    {
        if (_host is null) return;

        if (key != "subscriptions" && await _guard.IsExpiredAsync())
            key = "subscriptions";

        Page page = key switch
        {
            "dashboard" => _sp.GetRequiredService<DashboardPage>(),
            "clients" => _sp.GetRequiredService<ClientPage>(),
            "workorders" => _sp.GetRequiredService<WorkorderPage>(),
            "subscriptions" => _sp.GetRequiredService<SubscriptionPage>(),
            _ => _sp.GetRequiredService<DashboardPage>()
        };

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            _host.Detail = new NavigationPage(page);
            _host.IsPresented = false;
        });
    }

    // ✅ NAVEGAÇÃO DENTRO DA SEÇÃO: empilha página (aí existe VOLTAR)
    public async Task PushAsync(string key)
    {
        if (await _guard.IsExpiredAsync())
        {
            await NavigateToAsync("subscriptions");
            return;
        }

        Page page = key switch
        {
            "createClient" => _sp.GetRequiredService<ClientCreatePage>(),
            "createWorkspace" => _sp.GetRequiredService<WorkorderCreatePage>(),
            "workspaceExecution" => _sp.GetRequiredService<WorkorderEditPage>(),
            _ => throw new ArgumentException($"Rota inválida: {key}")
        };

        var nav = GetNav();

        await MainThread.InvokeOnMainThreadAsync(async () =>
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
