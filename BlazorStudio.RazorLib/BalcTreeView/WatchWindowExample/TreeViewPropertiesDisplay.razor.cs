using BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.BalcTreeView.WatchWindowExample;

public partial class TreeViewPropertiesDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewProperties TreeViewProperties { get; set; } = null!;
}