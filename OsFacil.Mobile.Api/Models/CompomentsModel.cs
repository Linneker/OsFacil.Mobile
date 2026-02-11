using CommunityToolkit.Mvvm.ComponentModel;

namespace OsFacil.Mobile.Api.Models;

public partial class CompomentsModel : ObservableObject
{
    [ObservableProperty]
    private string name;
    [ObservableProperty]
    private string description;
    [ObservableProperty]
    private Page page;
}
