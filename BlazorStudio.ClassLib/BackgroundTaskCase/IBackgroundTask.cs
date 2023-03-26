namespace BlazorStudio.ClassLib.BackgroundTaskCase;

public interface IBackgroundTask
{
    public BackgroundTaskKey BackgroundTaskKey { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public Func<CancellationToken, Task> WorkItem { get; init; }
    public Func<CancellationToken, Task> CancelFunc { get; init; }
}