namespace BlazorStudio.ClassLib.BackgroundTaskCase;

public interface IBackgroundTaskQueue  
{  
    public void QueueBackgroundWorkItem(
        IBackgroundTask backgroundTask);
    
    public Task<IBackgroundTask?> DequeueAsync(  
        CancellationToken cancellationToken);
}