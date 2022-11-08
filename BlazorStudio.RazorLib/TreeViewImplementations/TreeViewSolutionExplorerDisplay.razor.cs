using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.TreeViewImplementations;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeViewImplementations;

public partial class TreeViewSolutionExplorerDisplay : ComponentBase, ITreeViewNamespacePathRendererType
{
    [Parameter, EditorRequired]
    public TreeViewNamespacePath TreeViewNamespacePath { get; set; } = null!;
}