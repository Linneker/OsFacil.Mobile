using CommunityToolkit.Mvvm.ComponentModel;

namespace OsFacil.Mobile.Api.Models.Dashboard;

public partial class DashboardModel : ObservableObject
{
    [ObservableProperty] private int totalWorkOrders;
    [ObservableProperty] private int workOrdersThisMonth;
    [ObservableProperty] private int workOrdersToday;
    [ObservableProperty] private string currentPlan;
    [ObservableProperty] private int planLimit;
    [ObservableProperty] private int planUsed;
    [ObservableProperty] private decimal mrr;
    [ObservableProperty] private double conversionRate;
}
