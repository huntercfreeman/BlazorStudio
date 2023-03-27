using BlazorCommon.RazorLib.Notification;
using BlazorCommon.RazorLib.Store.NotificationCase;
using BlazorStudio.ClassLib.CommonComponents;
using Fluxor;

namespace BlazorStudio.ClassLib.BackgroundTaskCase;

public class BackgroundTaskMonitor : IBackgroundTaskMonitor
{
    private readonly INotificationService _notificationService;
    private readonly ICommonComponentRenderers _commonComponentRenderers;

    public BackgroundTaskMonitor(
        INotificationService notificationService,
        ICommonComponentRenderers commonComponentRenderers)
    {
        _notificationService = notificationService;
        _commonComponentRenderers = commonComponentRenderers;
    }
    
    public IBackgroundTask? ExecutingBackgroundTask { get; private set; }
    
    public event Action? ExecutingBackgroundTaskChanged;

    public void SetExecutingBackgroundTask(IBackgroundTask? backgroundTask)
    {
        ExecutingBackgroundTask = backgroundTask;
        ExecutingBackgroundTaskChanged?.Invoke();

        if (backgroundTask is not null)
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
                null);
        
            backgroundTask.Dispatcher.Dispatch(
                new NotificationRecordsCollection.RegisterAction(
                    notificationRecord));
        }
    }
}