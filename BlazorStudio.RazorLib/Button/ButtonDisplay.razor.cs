using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Button;

public partial class ButtonDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public Action OnClickAction { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;
    [Parameter]
    public bool IsDisabled { get; set; }
    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;

    private string IsDisabledCssClass => IsDisabled
        ? "bstudio_disabled"
        : string.Empty;

    public void FireOnClickAction()
    {
        if (!IsDisabled)
            OnClickAction.Invoke();
    }
}