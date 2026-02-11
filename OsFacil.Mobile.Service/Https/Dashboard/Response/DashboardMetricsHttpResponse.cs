using OsFacil.Mobile.Service.Util;
using System;
using System.Text.Json.Serialization;

namespace OsFacil.Mobile.Service.Https.Dashboard.Response;

public sealed class DashboardMetricsHttpResponse : IResponseHttp
{
    [JsonPropertyName("totalWorkOrders")]
    public int TotalWorkOrders { get; set; }

    [JsonPropertyName("workOrdersThisMonth")]
    public int WorkOrdersThisMonth { get; set; }

    [JsonPropertyName("workOrdersToday")]
    public int WorkOrdersToday { get; set; }

    [JsonPropertyName("currentPlan")]
    public string CurrentPlan { get; set; }

    [JsonPropertyName("planLimit")]
    public int PlanLimit { get; set; }

    [JsonPropertyName("planUsed")]
    public int PlanUsed { get; set; }

    [JsonPropertyName("mrr")]
    public decimal Mrr { get; set; }

    [JsonPropertyName("conversionRate")]
    public double ConversionRate { get; set; }

    public DashboardMetricsHttpResponse() { }

    public DashboardMetricsHttpResponse(
        int totalWorkOrders,
        int workOrdersThisMonth,
        int workOrdersToday,
        string currentPlan,
        int planLimit,
        int planUsed,
        decimal mrr,
        double conversionRate)
    {
        TotalWorkOrders = totalWorkOrders;
        WorkOrdersThisMonth = workOrdersThisMonth;
        WorkOrdersToday = workOrdersToday;
        CurrentPlan = currentPlan;
        PlanLimit = planLimit;
        PlanUsed = planUsed;
        Mrr = mrr;
        ConversionRate = conversionRate;
    }
}

