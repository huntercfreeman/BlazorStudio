using BlazorStudio.ClassLib.Store.NotificationCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Notification;

public partial class TestNotificationDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public NotificationRecord NotificationRecord { get; set; } = null!;

    protected void CloseOnClick()
    {
        Dispatcher.Dispatch(new DisposeNotificationAction(NotificationRecord));
    }
}