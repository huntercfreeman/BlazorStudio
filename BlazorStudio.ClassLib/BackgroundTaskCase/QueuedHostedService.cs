using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlazorStudio.ClassLib.BackgroundTaskCase;

public class QueuedHostedService : BackgroundService
{  
    private readonly ILogger _logger;  
  
    public QueuedHostedService(
        IBackgroundTaskQueue taskQueue,  
        IBackgroundTaskMonitor taskMonitor,  
        ILoggerFactory loggerFactory)  
    {  
        TaskQueue = taskQueue;
        TaskMonitor = taskMonitor;
        _logger = loggerFactory.CreateLogger<QueuedHostedService>();  
    }  
  
    public IBackgroundTaskQueue TaskQueue { get; }  
    public IBackgroundTaskMonitor TaskMonitor { get; }  
  
    protected async override Task ExecuteAsync(  
        CancellationToken cancellationToken)  
    {  
        _logger.LogInformation("Queued Hosted Service is starting.");  
  
        while (!cancellationToken.IsCancellationRequested)  
        {  
            var backgroundTask = await TaskQueue
                .DequeueAsync(cancellationToken);

            if (backgroundTask is not null)
            {
                try
                {
                    TaskMonitor.SetExecutingBackgroundTask(backgroundTask);
                    await backgroundTask.InvokeWorkItem(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred executing {WorkItem}.", nameof(backgroundTask));
                }
                finally
                {
                    TaskMonitor.SetExecutingBackgroundTask(null);
                }
            }
        }  
  
        _logger.LogInformation("Queued Hosted Service is stopping.");  
    }  
}