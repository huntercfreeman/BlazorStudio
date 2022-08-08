using BlazorStudio.ClassLib.Store.NotificationCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Notification;

public partial class NotificationDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public NotificationRecord NotificationRecord { get; set; } = null!;

    private void DismissOnClick()
    {
        Dispatcher.Dispatch(new DisposeNotificationAction(NotificationRecord));
    }
}