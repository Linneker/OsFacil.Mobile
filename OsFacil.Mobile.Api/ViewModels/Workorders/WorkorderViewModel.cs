using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OsFacil.Mobile.Api.Models.Workorders;
using OsFacil.Mobile.Api.Services.Billing;
using OsFacil.Mobile.Api.Services.Navigation;
using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Api.ViewModels.Messages;
using OsFacil.Mobile.Api.ViewModels.Workorders.Messages;
using OsFacil.Mobile.Service.Https.Workorders;
using OsFacil.Mobile.Service.Https.Workorders.Request;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace OsFacil.Mobile.Api.ViewModels.Workorders;

public partial class WorkorderViewModel : ObservableObject
{
    private readonly IWorkspaceHttp _service;
    private readonly IAuthSession _session;
    private readonly IFlyoutNavigationService _nav;
    private readonly WorkorderEditViewModel _editVm;
    private readonly ISubscriptionGuard _guard;

    private const int PageSize = 20;
    private int _page = 1;
    private bool _hasMore = true;
    private GetWorkOrdersPaginatedRequest _getWorkOrdersPaginatedRequest = new GetWorkOrdersPaginatedRequest();

    public IRelayCommand<WorkorderModel> OpenExecutionCommand { get; }

    [ObservableProperty] private ObservableCollection<WorkorderModel> items = new();

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isRefreshing;

    [ObservableProperty] private bool hasLoaded;
    [ObservableProperty] private bool needsReload;

    [ObservableProperty] private bool isFiltersOpen;
    public string FiltersToggleText => IsFiltersOpen ? "Fechar" : "Abrir";

    [ObservableProperty] private WorkorderFiltersModel filters = new();
    public WorkorderViewModel(IWorkspaceHttp service, IAuthSession session, IFlyoutNavigationService nav, WorkorderEditViewModel editVm, ISubscriptionGuard guard)
    {
        _service = service;
        _session = session;
        _nav = nav;
        _editVm = editVm;
        _guard = guard;

        OpenExecutionCommand = new AsyncRelayCommand<WorkorderModel>(OpenExecutionAsync);

        WeakReferenceMessenger.Default.Register<WorkordersChangedMessage>(this, (r, m) =>
        {
            ((WorkorderViewModel)r).NeedsReload = m.Value;
        });

        WeakReferenceMessenger.Default.Register<SessionClearedMessage>(this, (r, _) =>
        {
            var vm = (WorkorderViewModel)r;
            vm.Items.Clear();
            vm.HasLoaded = false;
            vm.NeedsReload = false;
            vm._page = 1;
            vm._hasMore = true;
            vm._getWorkOrdersPaginatedRequest = new GetWorkOrdersPaginatedRequest();
            vm.Filters = new WorkorderFiltersModel();
            vm.IsFiltersOpen = false;
        });
    }

    partial void OnIsFiltersOpenChanged(bool value) => OnPropertyChanged(nameof(FiltersToggleText));

    [RelayCommand]
    private void ToggleFilters()
    {
        IsFiltersOpen = !IsFiltersOpen;
    }

    [RelayCommand]
    private async Task ApplyFiltersAsync()
    {
        _getWorkOrdersPaginatedRequest.ClientNameOrSlug = string.IsNullOrWhiteSpace(Filters.ClientNameOrSlug) ? null : Filters.ClientNameOrSlug.Trim();

        // Amount
        if (TryParseDecimal(Filters.AmountText, out var amount))
            _getWorkOrdersPaginatedRequest.Amount = amount;
        else
            _getWorkOrdersPaginatedRequest.Amount = null;

        // Datas: só aplica se a flag estiver marcada
        if (Filters.UseCreatedAtRange)
        {
            _getWorkOrdersPaginatedRequest.StartCreatedAt = Filters.StartCreatedAtDate.Date;
            _getWorkOrdersPaginatedRequest.FinishedCreatedAt = Filters.FinishedCreatedAtDate.Date.AddDays(1).AddTicks(-1);
        }
        else
        {
            _getWorkOrdersPaginatedRequest.StartCreatedAt = null;
            _getWorkOrdersPaginatedRequest.FinishedCreatedAt = null;
        }

        if (Filters.UseFinishedAtRange)
        {
            _getWorkOrdersPaginatedRequest.StartFinishedAt = Filters.StartFinishedAtDate.Date;
            _getWorkOrdersPaginatedRequest.FinesedFinishedAt = Filters.FinesedFinishedAtDate.Date.AddDays(1).AddTicks(-1);
        }
        else
        {
            _getWorkOrdersPaginatedRequest.StartFinishedAt = null;
            _getWorkOrdersPaginatedRequest.FinesedFinishedAt = null;
        }

        // fecha e recarrega
        IsFiltersOpen = false;
        await ReloadAsync();
    }

    [RelayCommand]
    private async Task ClearFiltersAsync()
    {
        Filters.AmountText = "";
        Filters.ClientNameOrSlug = "";
        
        Filters.UseCreatedAtRange = false;
        Filters.UseFinishedAtRange = false;

        // mantém datepickers em hoje, mas sem aplicar filtro
        Filters.StartCreatedAtDate = DateTime.Today;
        Filters.FinishedCreatedAtDate = DateTime.Today;
        Filters.StartFinishedAtDate = DateTime.Today;
        Filters.FinesedFinishedAtDate = DateTime.Today;
        _getWorkOrdersPaginatedRequest = new();
        await ReloadAsync();
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
            IsRefreshing = true;
            await ReloadAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    public async Task ReloadAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            if (await _guard.IsExpiredAsync())
            {
                await _nav.NavigateToAsync("subscriptions");
                return;
            }

            Items.Clear();
            _page = 1;
            _hasMore = true;

            await LoadMoreCoreAsync();

            HasLoaded = true;
            NeedsReload = false;
        }
        finally
        {
            IsBusy = false;
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
        var token = _session.AccessToken ?? "";
        _getWorkOrdersPaginatedRequest.PageSize = PageSize;
        _getWorkOrdersPaginatedRequest.PageNumber = _page;
        // ✅ agora usa filtros completos, sem SearchText
        var result = await _service.GetWorkOrdersPaginatedAsync
            (token, _getWorkOrdersPaginatedRequest);
        if (result.Data is not null)
        {
            foreach (var w in result.Data)
            {
                Items.Add(new WorkorderModel
                {
                    Id = w.Id,
                    ClientName = w.ClientName,
                    Title = w.Title,
                    Status = ConverStatusTo(w.Status),
                    Amount = w.Amount ?? 0m
                });
            }
            _page++;
            _hasMore = result.Data.Count == PageSize;
        }
    }

    private string ConverStatusTo(int status)
    {
        switch (status)
        {
            case (int)WorkOrderStatus.Open:
                return "Aberto";
            case (int)WorkOrderStatus.InProgress:
                return "Em andamento";
            case (int)WorkOrderStatus.Done:
                return "Concluido";
            case (int)WorkOrderStatus.Cancelled:
                return "Cancelado";
            default:
                return "Desconecido";
        }
    }

    [RelayCommand]
    private Task AddWorkspaceAsync()
    {
        return _nav.PushAsync("createWorkspace");
    }

    private async Task OpenExecutionAsync(WorkorderModel? item)
    {
        if (item is null) return;

        _editVm.PrepareLoad(item.Id);
        await _nav.PushAsync("workspaceExecution");
    }

    private static bool TryParseDecimal(string? text, out decimal value)
    {
        value = 0m;
        if (string.IsNullOrWhiteSpace(text)) return false;

        // aceita "150,00" ou "150.00"
        var t = text.Trim();

        if (decimal.TryParse(t, NumberStyles.Number, new CultureInfo("pt-BR"), out value))
            return true;

        if (decimal.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
            return true;

        return false;
    }

}
public enum WorkOrderStatus { Open = 0, InProgress = 1, Done = 2, Cancelled = 3 }
