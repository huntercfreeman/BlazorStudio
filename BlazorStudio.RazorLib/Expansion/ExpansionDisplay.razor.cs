using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Expansion;

public partial class ExpansionDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public RenderFragment TitleRenderFragment { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment BodyRenderFragment { get; set; } = null!;
    [Parameter]
    public bool OnClickShouldToggleIsExpanded { get; set; } = true;
    [Parameter]
    public bool ShowIsExpandedChevron { get; set; } = true;
    [Parameter]
    public bool RenderSeparatorBetweenChevronAndTitle { get; set; } = true;
    [Parameter]
    public string StyleCssString { get; set; } = string.Empty;
    [Parameter]
    public string TitleCssClass { get; set; } = string.Empty;

    public bool IsExpanded { get; private set; }

    public void ToggleIsExpanded()
    {
        IsExpanded = !IsExpanded;

        InvokeAsync(StateHasChanged);
    }
}