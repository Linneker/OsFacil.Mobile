using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OsFacil.Mobile.Api.Services.Navigation;
using System.Collections.ObjectModel;

namespace OsFacil.Mobile.Api.ViewModels;

public partial class MenuViewModel : ObservableObject
{
    private readonly IFlyoutNavigationService _nav;
    private readonly IRootNavigator _root;

    public ObservableCollection<MenuItemVm> Items { get; } = new()
    {
        new("dashboard",  "Dashboard"),
        new("clients",    "Clientes"),
        new("workorders", "Ordens de serviço"),
        new("subscriptions", "Assinaturas"),
        new("logout",     "Sair"),
    };

    public MenuViewModel(IFlyoutNavigationService nav, IRootNavigator root)
    {
        _nav = nav;
        _root = root;
    }

    [RelayCommand]
    private Task SelectAsync(MenuItemVm item)
    {
        if (item.Key == "logout")
            return _root.Logout();

        return _nav.NavigateToAsync(item.Key);
    }



}
public sealed record MenuItemVm(string Key, string Title);
