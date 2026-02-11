using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OsFacil.Mobile.Api.Services.Navigation;
using OsFacil.Mobile.Api.ViewModels.Clients;
using System.Collections.ObjectModel;

namespace OsFacil.Mobile.Api.ViewModels;

public partial class MenuViewModel : ObservableObject
{
    private readonly IFlyoutNavigationService _nav;

    private readonly ClientViewModel _clientViewModel;
    public ObservableCollection<MenuItemVm> Items { get; } = new()
    {
        new("dashboard","Dashboard"),
        new("clients","Clientes"),
        new("workorders","Ordens de serviço"),
        new("subscriptions","Assinaturas"),
        new("profile","Perfil"),
    };

    public MenuViewModel(IFlyoutNavigationService nav)
    {
        _nav = nav;
    }

    [RelayCommand]
    private Task SelectAsync(MenuItemVm item)
    => _nav.NavigateToAsync(item.Key);



}
public sealed record MenuItemVm(string Key, string Title);
