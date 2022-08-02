using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.DebuggingHelp;

public partial class DebugExpansion : ComponentBase
{
    [Parameter, EditorRequired]
    public string Title { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;

    private bool _isExpanded;
}