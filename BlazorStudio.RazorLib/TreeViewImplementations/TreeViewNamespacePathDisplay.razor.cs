using BlazorStudio.ClassLib.ComponentRenderers.Types;
using BlazorStudio.ClassLib.Namespaces;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeViewImplementations;

public partial class TreeViewNamespacePathDisplay 
    : ComponentBase, ITreeViewNamespacePathRendererType
{
    [Parameter, EditorRequired]
    public NamespacePath NamespacePath { get; set; } = null!;
}