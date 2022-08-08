using BlazorStudio.ClassLib.Store.NotificationCase;
using Fluxor.Blazor.Web.Components;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Notification;

public partial class NotificationsInitializer : FluxorComponent
{
    [Inject]
    private IState<NotificationStates> NotificationStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    protected override void Dispose(bool disposing)
    {
        var notificationStates = NotificationStatesWrap.Value;

        foreach (var notification in notificationStates.List)
        {
            Dispatcher.Dispatch(new DisposeNotificationAction(notification));
        }

        base.Dispose(disposing);
    }
}