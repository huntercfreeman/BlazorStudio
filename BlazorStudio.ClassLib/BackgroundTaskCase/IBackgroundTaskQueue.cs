namespace BlazorStudio.ClassLib.BackgroundTaskCase;

public interface IBackgroundTaskQueue  
{  
    public void QueueBackgroundWorkItem(
        IBackgroundTask backgroundTask);
    
    public void QueueBackgroundWorkItem(
        BackgroundTaskKey backgroundTaskKey,
        string name,
        string description,
        Func<CancellationToken, Task> workItem,
        Func<CancellationToken, Task> cancelFunc);  
  
    public Task<IBackgroundTask>? DequeueAsync(  
        CancellationToken cancellationToken);
}