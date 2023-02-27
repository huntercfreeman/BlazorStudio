using BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.BalcTreeView.WatchWindowExample;

public partial class TreeViewInterfaceImplementationDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewInterfaceImplementation TreeViewInterfaceImplementation { get; set; } = null!;
}