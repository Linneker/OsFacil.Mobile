using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace OsFacil.Mobile.Api.Models;

public partial class LoginModel : ObservableObject
{
    [ObservableProperty]
    private string? slug;
    [ObservableProperty] 
    private string? email;
    [ObservableProperty] 
    private string? password;
    [ObservableProperty] 
    private string? error;



}
