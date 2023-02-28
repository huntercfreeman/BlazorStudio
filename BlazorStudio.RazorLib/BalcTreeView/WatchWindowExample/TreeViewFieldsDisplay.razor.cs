using BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.BalcTreeView.WatchWindowExample;

public partial class TreeViewFieldsDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewFields TreeViewFields { get; set; } = null!;
}