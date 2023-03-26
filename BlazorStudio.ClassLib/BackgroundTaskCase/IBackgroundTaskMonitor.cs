namespace BlazorStudio.ClassLib.BackgroundTaskCase;

public interface IBackgroundTaskMonitor
{
    public IBackgroundTask? ExecutingBackgroundTask { get; }
    
    public event Action? ExecutingBackgroundTaskChanged;

    public void SetExecutingBackgroundTask(IBackgroundTask? backgroundTask);
}