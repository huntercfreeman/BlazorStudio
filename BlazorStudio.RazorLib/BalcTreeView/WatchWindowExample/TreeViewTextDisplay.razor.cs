using BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.BalcTreeView.WatchWindowExample;

public partial class TreeViewTextDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewText TreeViewText { get; set; } = null!;
}