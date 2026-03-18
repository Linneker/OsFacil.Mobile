using CommunityToolkit.Mvvm.ComponentModel;

namespace OsFacil.Mobile.Api.Models.Billing;

public partial class SubscriptionModel : ObservableObject
{
    // Status atual
    [ObservableProperty]
    private string plan = string.Empty;

    [ObservableProperty]
    private string status = string.Empty;

    [ObservableProperty]
    private bool expired;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ValidUntilFormatted))]
    private DateTime? validUntil;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MonthlyLimitText))]
    private int monthlyLimit;

    [ObservableProperty]
    private int usedThisMonth;

    // Seleção de renovação
    [ObservableProperty]
    private int selectedPlanIndex;

    [ObservableProperty]
    private int months = 1;

    [ObservableProperty]
    private int flexQuantity = 200;

    [ObservableProperty]
    private bool isFlexSelected;

    [ObservableProperty]
    private decimal totalPrice;

    [ObservableProperty]
    private decimal unitPrice;

    [ObservableProperty]
    private string selectedPlanName = "Basic";

    public string ValidUntilFormatted => ValidUntil?.ToString("dd/MM/yyyy") ?? "—";

    public string MonthlyLimitText => MonthlyLimit == 0 ? "Ilimitado" : MonthlyLimit.ToString();

    partial void OnSelectedPlanIndexChanged(int value)
    {
        IsFlexSelected = value == 1;
        SelectedPlanName = value switch
        {
            0 => "Basic",
            1 => "Flex",
            2 => "Pro",
            _ => "Basic"
        };
        RecalculatePrice();
    }

    partial void OnMonthsChanged(int value) => RecalculatePrice();

    partial void OnFlexQuantityChanged(int value)
    {
        if (value < 200)
        {
            FlexQuantity = 200;
            return;
        }
        RecalculatePrice();
    }

    public void RecalculatePrice()
    {
        UnitPrice = SelectedPlanIndex switch
        {
            0 => 59.00m, // Basic
            1 => 65.00m + ((FlexQuantity - 200) / 50m) * 6.25m, // Flex
            2 => 99.00m, // Pro
            _ => 59.00m
        };
        TotalPrice = UnitPrice * Months;
    }

    public int GetWorkOrderLimit() => SelectedPlanIndex switch
    {
        0 => 150,
        1 => FlexQuantity,
        2 => 0, // ilimitado
        _ => 150
    };
}
