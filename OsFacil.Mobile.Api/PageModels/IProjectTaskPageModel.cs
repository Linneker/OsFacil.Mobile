using CommunityToolkit.Mvvm.Input;
using OsFacil.Mobile.Api.Models;

namespace OsFacil.Mobile.Api.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}