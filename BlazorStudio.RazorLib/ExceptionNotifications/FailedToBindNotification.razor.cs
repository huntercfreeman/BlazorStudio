using BlazorStudio.ClassLib.Store.NotificationCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.ExceptionNotifications;

public partial class FailedToBindNotification : ComponentBase
{
    [CascadingParameter]
    public NotificationRecord NotificationRecord { get; set; } = null!;
}