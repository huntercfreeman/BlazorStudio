using BlazorStudio.ClassLib.CommonComponents;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Notifications;

public partial class CommonInformativeNotificationDisplay 
    : ComponentBase, IInformativeNotificationRendererType
{
    [Parameter, EditorRequired]
    public string Message { get; set; } = null!;
}