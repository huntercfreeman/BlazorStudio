using BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.BalcTreeView.WatchWindowExample;

public partial class TreeViewExceptionDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewException TreeViewException { get; set; } = null!;
}