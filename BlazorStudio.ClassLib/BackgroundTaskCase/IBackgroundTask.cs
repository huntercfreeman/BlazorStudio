using Fluxor;

namespace BlazorStudio.ClassLib.BackgroundTaskCase;

public interface IBackgroundTask
{
    public BackgroundTaskKey BackgroundTaskKey { get; }
    public string Name { get; }
    public string Description { get; }
    public Task? WorkProgress { get; }
    public Func<CancellationToken, Task> CancelFunc { get; }
    public IDispatcher? Dispatcher { get; }
    
    public Task InvokeWorkItem(CancellationToken cancellationToken);
}