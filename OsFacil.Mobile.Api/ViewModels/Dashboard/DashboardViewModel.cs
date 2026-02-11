using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OsFacil.Mobile.Api.Models.Dashboard;
using OsFacil.Mobile.Api.Services.Session;
using OsFacil.Mobile.Service.Https.Dashboard;
using System.Collections.ObjectModel;

namespace OsFacil.Mobile.Api.ViewModels;

public sealed partial class DashboardViewModel : ObservableObject
{
    private readonly IMetricsHttp _metrics;
    private readonly IAuthSession _session;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isRefreshing;
    [ObservableProperty] private bool hasLoaded;

    [ObservableProperty] private DashboardModel dashboard = new();
    [ObservableProperty] public ObservableCollection<PlanSliceModel> planSlices;

    public int PlanRemaining => Math.Max(dashboard.PlanLimit - dashboard.PlanUsed, 0);

    // 0..100
    public double PlanUsagePercent => dashboard.PlanLimit <= 0 ? 0 : (double)dashboard.PlanUsed / dashboard.PlanLimit * 100.0;

    public string PlanUsageText => dashboard.PlanLimit <= 0 ? "0 / 0" : $"{dashboard.PlanUsed} / {dashboard.PlanLimit}";
    public string MrrText => $"{dashboard.Mrr:C}";
    public string ConversionRateText => $"{dashboard.ConversionRate:0.##}%";

    public DashboardViewModel(IMetricsHttp metrics, IAuthSession session)
    {
        _metrics = metrics;
        _session = session;
        PlanSlices = new ObservableCollection<PlanSliceModel>();
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var dto = await _metrics.GetDashboardAsync(_session.AccessToken ?? string.Empty);
            if (!dto.IsSuccessStatusCode)
            {

            }
            else
            {
                dashboard.TotalWorkOrders = dto.Data.TotalWorkOrders;
                dashboard.WorkOrdersThisMonth = dto.Data.WorkOrdersThisMonth;
                dashboard.WorkOrdersToday = dto.Data.WorkOrdersToday;

                dashboard.CurrentPlan = dto.Data.CurrentPlan;
                dashboard.PlanLimit = dto.Data.PlanLimit;
                dashboard.PlanUsed = dto.Data.PlanUsed;

                dashboard.Mrr = dto.Data.Mrr;
                dashboard.ConversionRate = dto.Data.ConversionRate;

                UpdatePlanSlices();

                HasLoaded = true;

                // propriedades derivadas
                OnPropertyChanged(nameof(PlanRemaining));
                OnPropertyChanged(nameof(PlanUsagePercent));
                OnPropertyChanged(nameof(PlanUsageText));
                OnPropertyChanged(nameof(MrrText));
                OnPropertyChanged(nameof(ConversionRateText));
            }
        }
        finally
        {
            IsBusy = false;
        }
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

    private void UpdatePlanSlices()
    {
        PlanSlices.Clear();

        var used = Math.Max(dashboard.PlanUsed, 0);
        var remaining = Math.Max(dashboard.PlanLimit - dashboard.PlanUsed, 0);

        // 2 fatias: usado vs restante
        PlanSlices.Add(new PlanSliceModel("Usado", used));
        PlanSlices.Add(new PlanSliceModel("Restante", remaining == 0 && dashboard.PlanLimit == 0 ? 1 : remaining)); // evita “chart vazio”
    }
}

public sealed record PlanSliceModel(string Label, int Value);
