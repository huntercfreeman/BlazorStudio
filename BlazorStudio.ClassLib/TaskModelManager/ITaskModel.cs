using BlazorStudio.ClassLib.Errors;

namespace BlazorStudio.ClassLib.TaskModelManager;

public interface ITaskModel
{
    public TaskModelKey Key { get; }
    public Task Task { get; }
    public TimeSpan LifetimeUntilSeemsUnresponsive { get; }
    public RichErrorModel? RichErrorModel { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; }
    public bool IsBackgroundTask { get; }
}