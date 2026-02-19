using CommunityToolkit.Mvvm.ComponentModel;

namespace OsFacil.Mobile.Api.Models;

public partial class RegisterModel : ObservableObject
{
    [ObservableProperty]
    private string? companyName;

    [ObservableProperty]
    private string? companySlug;

    [ObservableProperty]
    private string? adminName;

    [ObservableProperty]
    private string? adminEmail;

    [ObservableProperty]
    private string? adminPassword;

    [ObservableProperty]
    private string? error;
}
