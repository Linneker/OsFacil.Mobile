using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using OsFacil.Mobile.Api.Models;
using OsFacil.Mobile.Api.Models.Clients;
using OsFacil.Mobile.Api.Services.Navigation;
using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Api.ViewModels.Clients.Messagens;
using OsFacil.Mobile.Api.Views.Clients;
using OsFacil.Mobile.Service.Https.Clients;
using System.Collections.ObjectModel;

namespace OsFacil.Mobile.Api.ViewModels.Clients;

public partial class ClientViewModel : ObservableObject
{
    private readonly IClientHttp _service;
    private readonly IAuthSession _session;
    private readonly IFlyoutNavigationService _navagation;
    private readonly IToastService _toast;

    private const int PageSize = 20;
    private int _page = 1;
    private bool _hasMore = true;

    [ObservableProperty] private ObservableCollection<ClientModel> items;
    [ObservableProperty] private string? searchText = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isRefreshing;
    [ObservableProperty] private bool hasLoaded;
    [ObservableProperty] private bool needsReload;

    public ClientViewModel(IClientHttp service, IAuthSession session, IFlyoutNavigationService sp, IToastService toast)
    {
        _service = service;
        _session = session;
        _navagation = sp;

        WeakReferenceMessenger.Default.Register<ClientsChangedMessage>(this, (obj, msg) =>
        {
            NeedsReload = msg.Value; // só marca, não recarrega na hora
        });
        _toast = toast;
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = ReloadAsync();
    }


    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (IsBusy)
        {
            IsRefreshing = false;
            return;
        }

        try
        {
            IsBusy = true;
            IsRefreshing = true;
            await ReloadCoreAsync();
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
            HasLoaded = true;
            NeedsReload = false;

        }
    }
    private async Task ReloadCoreAsync()
    {
        Items.Clear();

        var result = await _service.GetClientsAsync(_session.AccessToken ?? "", SearchText, page: _page, pageSize: PageSize);
        if (result.Data is not null)
        {
            foreach (var c in result.Data)
                Items.Add(new ClientModel() {Id =c.Id, Email = c.Email, Name = c.Name, Phone = c.Phone });

            _page++;
            _hasMore = result.Data.Count == PageSize;
        }
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            if (Items is null)
                Items = new ObservableCollection<ClientModel>();
            else
                Items.Clear();

            _page = 0;
            _hasMore = true;

            await LoadMoreCoreAsync();
        }
        finally
        {
            IsBusy = false;
            HasLoaded = true;
            NeedsReload = false;
        }
    }

    [RelayCommand]
    private async Task LoadMoreAsync()
    {
        if (IsBusy) return;
        if (!_hasMore) return;

        try
        {
            IsBusy = true;
            await LoadMoreCoreAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadMoreCoreAsync()
    {
        if (string.IsNullOrWhiteSpace(_session.AccessToken))
        {

        }
        var result = await _service.GetClientsAsync(_session.AccessToken ?? "", SearchText ?? "", page: _page, pageSize: PageSize);
        if (Items is null)
            Items = new();
        if (result.Data is not null)
        {
            foreach (var c in result.Data)
                Items.Add(new ClientModel() { Id = c.Id, Email = c.Email, Name = c.Name, Phone = c.Phone });

            _page++;
            _hasMore = result.Data.Count == PageSize;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync(ClientModel item)
    {
        if (item is null) return;

        var result = await _service.DeleteClientsAsync(_session.AccessToken ?? "", item.Id, item.Name);
        await _toast.ShowAsync(result.IsSuccessStatusCode ? result.Data.Message : result.Error);
        if(result.IsSuccessStatusCode)
            Items.Remove(item);
    }

    [RelayCommand]
    private async Task AddClientAsync()
    {
        await _navagation.PushAsync("createClient");
    }

}
