using BlazorStudio.ClassLib.Renderer;
using BlazorStudio.ClassLib.Store.NotificationCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Notification;

public partial class DefaultInformationRenderer : ComponentBase, IDefaultInformationRenderer
{
    [CascadingParameter]
    [EditorRequired]
    public NotificationRecord NotificationRecord { get; set; } = null!;
}