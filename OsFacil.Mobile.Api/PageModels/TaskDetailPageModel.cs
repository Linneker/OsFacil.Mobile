using CommunityToolkit.Mvvm.ComponentModel;
using OsFacil.Mobile.Api.Models;

namespace OsFacil.Mobile.Api.PageModels;

public partial class TaskDetailPageModel : ObservableObject
{
    [ObservableProperty]
    private ProjectTask task = new();
}
