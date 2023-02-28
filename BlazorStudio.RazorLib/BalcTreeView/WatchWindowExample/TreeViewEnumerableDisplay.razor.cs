using BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.BalcTreeView.WatchWindowExample;

public partial class TreeViewEnumerableDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewEnumerable TreeViewEnumerable { get; set; } = null!;
}