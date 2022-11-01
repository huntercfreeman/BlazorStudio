using BlazorStudio.ClassLib.Store.NotificationCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Notifications;

public partial class NotificationInitializer : FluxorComponent
{
    [Inject]
    private IState<NotificationState> NotificationStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    protected override void Dispose(bool disposing)
    {
        var notificationState = NotificationStateWrap.Value;

        foreach (var notification in notificationState.Notifications)
        {
            Dispatcher.Dispatch(
                new NotificationState.DisposeNotificationAction(
                    notification.NotificationKey));
        }
        
        base.Dispose(disposing);
    }
}