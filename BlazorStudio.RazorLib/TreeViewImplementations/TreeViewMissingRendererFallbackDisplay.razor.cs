using BlazorCommon.RazorLib.ComponentRenderers.Types;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeViewImplementations;

public partial class TreeViewMissingRendererFallbackDisplay
    : ComponentBase, ITreeViewMissingRendererFallbackType
{
    [Parameter, EditorRequired]
    public string DisplayText { get; set; } = string.Empty;
}