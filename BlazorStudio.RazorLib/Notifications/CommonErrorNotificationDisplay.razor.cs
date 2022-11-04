using BlazorStudio.ClassLib.CommonComponents;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Notifications;

public partial class CommonErrorNotificationDisplay 
    : ComponentBase, IErrorNotificationRendererType
{
    [Parameter, EditorRequired]
    public string Message { get; set; } = null!;
}