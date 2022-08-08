using BlazorStudio.ClassLib.Store.NotificationCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Notification;

public partial class NotificationDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public NotificationRecord NotificationRecord { get; set; } = null!;
}