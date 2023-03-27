using BlazorCommon.RazorLib.Notification;
using BlazorCommon.RazorLib.Store.NotificationCase;
using BlazorStudio.ClassLib.CommonComponents;
using Fluxor;

namespace BlazorStudio.ClassLib.BackgroundTaskCase;

public class BackgroundTaskMonitor : IBackgroundTaskMonitor
{
    private readonly ICommonComponentRenderers _commonComponentRenderers;

    public BackgroundTaskMonitor(
        ICommonComponentRenderers commonComponentRenderers)
    {
        _commonComponentRenderers = commonComponentRenderers;
    }
    
    public IBackgroundTask? ExecutingBackgroundTask { get; private set; }
    
    public event Action? ExecutingBackgroundTaskChanged;

    public void SetExecutingBackgroundTask(IBackgroundTask? backgroundTask)
    {
        ExecutingBackgroundTask = backgroundTask;
        ExecutingBackgroundTaskChanged?.Invoke();

        if (backgroundTask?.Dispatcher is not null &&
            _commonComponentRenderers.BackgroundTaskDisplayRendererType is not null)
        {
            var notificationRecord = new NotificationRecord(
                NotificationKey.NewNotificationKey(),
                "ExecutingBackgroundTaskChanged",
                _commonComponentRenderers.BackgroundTaskDisplayRendererType,
                new Dictionary<string, object?>
                {
                    {
                        nameof(IBackgroundTaskDisplayRendererType.BackgroundTask),
                        backgroundTask
                    }
                },
                null,
                null);
        
            backgroundTask.Dispatcher.Dispatch(
                new NotificationRecordsCollection.RegisterAction(
                    notificationRecord));
        }
    }
}