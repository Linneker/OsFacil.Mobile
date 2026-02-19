using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace OsFacil.Mobile.Api.Models.Workorders;

public partial class EditWorkorderModel : ObservableObject
{
    [ObservableProperty] private Guid id;
    [ObservableProperty] private string clientId;
    [ObservableProperty] private string clientName;
    [ObservableProperty] private string title;
    [ObservableProperty] private string description;
    [ObservableProperty] private decimal amount;
    [ObservableProperty] private int selectedStatusIndex;
    [ObservableProperty] private ObservableCollection<ImageSource> beforeImages = new();
    [ObservableProperty] private ObservableCollection<ImageSource> afterImages = new();
    [ObservableProperty] private bool isLoading;

    public void Clean()
    {
        Id = Guid.Empty;
        ClientId = "";
        ClientName = "";
        Title = "";
        Description = "";
        Amount = 0;
        SelectedStatusIndex = 0;
        BeforeImages.Clear();
        AfterImages.Clear();
        IsLoading = false;
    }
}
