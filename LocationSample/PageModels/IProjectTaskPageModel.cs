using CommunityToolkit.Mvvm.Input;
using LocationSample.Models;

namespace LocationSample.PageModels;

public interface IProjectTaskPageModel
{
    IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
    bool IsBusy { get; }
}