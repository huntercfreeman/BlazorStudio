namespace BlazorStudio.ClassLib.BackgroundTaskCase;

public record BackgroundTask : IBackgroundTask
{
    public BackgroundTaskKey BackgroundTaskKey { get; init; } = BackgroundTaskKey.NewBackgroundTaskKey();
    public string Name { get; init; }
    public string Description { get; init; }
    public Func<CancellationToken, Task> WorkItem { get; init; }
    public Func<CancellationToken, Task> CancelFunc { get; init; }
}