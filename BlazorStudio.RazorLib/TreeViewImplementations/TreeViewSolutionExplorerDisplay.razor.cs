using BlazorStudio.ClassLib.TreeViewImplementations;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeViewImplementations;

public partial class TreeViewSolutionExplorerDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewSolutionExplorer TreeViewSolutionExplorer { get; set; } = null!;
}