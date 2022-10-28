using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Button;

public partial class ButtonDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public Action OnClickAction { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;
    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;
}