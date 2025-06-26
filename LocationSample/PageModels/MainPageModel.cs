using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocationSample.Models;
using LocationTracking.Abstractions;

namespace LocationSample.PageModels;

public partial class MainPageModel(
    SeedDataService seedDataService,
    ProjectRepository projectRepository,
    TaskRepository taskRepository,
    CategoryRepository categoryRepository,
    ModalErrorHandler errorHandler,
    ILocationTracker tracker,
    ILocationLogger logger)
    : ObservableObject, IProjectTaskPageModel
{
    private bool _isNavigatedTo;
    private bool _dataLoaded;

    [ObservableProperty] private List<CategoryChartData> _todoCategoryData = [];

    [ObservableProperty] private List<Brush> _todoCategoryColors = [];

    [ObservableProperty] private List<ProjectTask> _tasks = [];

    [ObservableProperty] private List<Project> _projects = [];

    [ObservableProperty] bool _isBusy;

    [ObservableProperty] bool _isRefreshing;

    [ObservableProperty] private string _today = DateTime.Now.ToString("dddd, MMM d");

    public bool HasCompletedTasks
        => Tasks?.Any(t => t.IsCompleted) ?? false;

    private async Task LoadData()
    {
        try
        {
            IsBusy = true;

            Projects = await projectRepository.ListAsync();

            var chartData = new List<CategoryChartData>();
            var chartColors = new List<Brush>();

            var categories = await categoryRepository.ListAsync();
            foreach (var category in categories)
            {
                chartColors.Add(category.ColorBrush);

                var ps = Projects.Where(p => p.CategoryID == category.ID).ToList();
                int tasksCount = ps.SelectMany(p => p.Tasks).Count();

                chartData.Add(new(category.Title, tasksCount));
            }

            TodoCategoryData = chartData;
            TodoCategoryColors = chartColors;

            Tasks = await taskRepository.ListAsync();
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(HasCompletedTasks));
        }
    }

    private async Task InitData(SeedDataService seedDataService)
    {
        bool isSeeded = Preferences.Default.ContainsKey("is_seeded");

        if (!isSeeded)
        {
            await seedDataService.LoadSeedDataAsync();
        }

        Preferences.Default.Set("is_seeded", true);
        await Refresh();
    }

    [RelayCommand]
    private async Task StartTracking()
    {
        await tracker.StartTrackingAsync();
    }

    [RelayCommand]
    private async Task StopTracking()
    {
        await tracker.StopTrackingAsync();
    }

    [RelayCommand]
    private async Task GetLogs()
    {
        var locations = await logger.GetAllLocationTraceAsync();
        var logsText = string.Join(Environment.NewLine, 
            locations.Select(loc => 
                $"{loc.Timestamp:g}: Lat {loc.Latitude:F4}, Long {loc.Longitude:F4}, Accuracy {loc.Accuracy:F4}, Altitude {loc.Altitude:F4}, Source {loc.Source}"));
        if (Shell.Current is { } shell)
            await shell.DisplayAlert("Location trace", logsText, "OK");
    }

    [RelayCommand]
    private async Task Refresh()
    {
        try
        {
            IsRefreshing = true;
            await LoadData();
        }
        catch (Exception e)
        {
            errorHandler.HandleError(e);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private void NavigatedTo() =>
        _isNavigatedTo = true;

    [RelayCommand]
    private void NavigatedFrom() =>
        _isNavigatedTo = false;

    [RelayCommand]
    private async Task Appearing()
    {
        if (!_dataLoaded)
        {
            await InitData(seedDataService);
            _dataLoaded = true;
            await Refresh();
        }
        // This means we are being navigated to
        else if (!_isNavigatedTo)
        {
            await Refresh();
        }
    }

    [RelayCommand]
    private Task TaskCompleted(ProjectTask task)
    {
        OnPropertyChanged(nameof(HasCompletedTasks));
        return taskRepository.SaveItemAsync(task);
    }

    [RelayCommand]
    private Task AddTask()
        => Shell.Current.GoToAsync($"task");

    [RelayCommand]
    private Task NavigateToProject(Project project)
        => Shell.Current.GoToAsync($"project?id={project.ID}");

    [RelayCommand]
    private Task NavigateToTask(ProjectTask task)
        => Shell.Current.GoToAsync($"task?id={task.ID}");

    [RelayCommand]
    private async Task CleanTasks()
    {
        var completedTasks = Tasks.Where(t => t.IsCompleted).ToList();
        foreach (var task in completedTasks)
        {
            await taskRepository.DeleteItemAsync(task);
            Tasks.Remove(task);
        }

        OnPropertyChanged(nameof(HasCompletedTasks));
        Tasks = new(Tasks);
        await AppShell.DisplayToastAsync("All cleaned up!");
    }
}