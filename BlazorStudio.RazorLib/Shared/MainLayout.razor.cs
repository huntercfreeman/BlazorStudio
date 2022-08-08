using BlazorStudio.ClassLib.Store.NotificationCase;
using BlazorStudio.ClassLib.Store.ThemeCase;
using BlazorStudio.RazorLib.Notification;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class MainLayout : FluxorLayout
{
    [Inject]
    private IState<ThemeState> ThemeStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private void TestNotificationOnClick()
    {
        var notificationRecord = new NotificationRecord(NotificationKey.NewNotificationKey(),
            "Test Notification",
            typeof(TestNotificationDisplay),
            null);

        Dispatcher.Dispatch(new RegisterNotificationAction(notificationRecord));
    }
}