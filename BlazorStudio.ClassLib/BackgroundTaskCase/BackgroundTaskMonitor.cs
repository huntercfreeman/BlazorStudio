namespace BlazorStudio.ClassLib.BackgroundTaskCase;

public class BackgroundTaskMonitor : IBackgroundTaskMonitor
{
    public IBackgroundTask? ExecutingBackgroundTask { get; private set; }
    
    public event Action? ExecutingBackgroundTaskChanged;

    public void SetExecutingBackgroundTask(IBackgroundTask backgroundTask)
    {
        ExecutingBackgroundTask = backgroundTask;
        ExecutingBackgroundTaskChanged?.Invoke();
    }
}