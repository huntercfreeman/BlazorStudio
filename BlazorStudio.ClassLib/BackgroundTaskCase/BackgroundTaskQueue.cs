using System.Collections.Concurrent;

namespace BlazorStudio.ClassLib.BackgroundTaskCase;

public class BackgroundTaskQueue : IBackgroundTaskQueue  
{  
    private readonly ConcurrentQueue<IBackgroundTask> _backgroundTasks =  new();  
    private readonly SemaphoreSlim _workItemsQueueSemaphoreSlim = new(0);  
  
    public void QueueBackgroundWorkItem(
        IBackgroundTask backgroundTask)  
    {
        _backgroundTasks.Enqueue(backgroundTask);

        _workItemsQueueSemaphoreSlim.Release();
    }

    public void QueueBackgroundWorkItem(
        BackgroundTaskKey backgroundTaskKey,
        string name,
        string description,
        Func<CancellationToken, Task> workItem,
        Func<CancellationToken, Task> cancelFunc)
    {
        QueueBackgroundWorkItem(
            new BackgroundTask
            {
                BackgroundTaskKey = backgroundTaskKey,
                Name = name,
                Description = description,
                WorkItem = workItem,
                CancelFunc = cancelFunc
            });
    }
  
    public async Task<IBackgroundTask>? DequeueAsync(  
        CancellationToken cancellationToken)
    {
        IBackgroundTask? backgroundTask;
        
        try
        {
            await _workItemsQueueSemaphoreSlim.WaitAsync(cancellationToken);
            
            _backgroundTasks.TryDequeue(out backgroundTask);
        }
        finally
        {
            _workItemsQueueSemaphoreSlim.Release();
        }
        
        return backgroundTask;  
    }  
}