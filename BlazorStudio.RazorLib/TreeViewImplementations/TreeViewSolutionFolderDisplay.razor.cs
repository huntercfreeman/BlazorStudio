using BlazorStudio.ClassLib.ComponentRenderers.Types;
using BlazorStudio.ClassLib.DotNet;
using BlazorStudio.ClassLib.Namespaces;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeViewImplementations;

public partial class TreeViewSolutionFolderDisplay 
    : ComponentBase, ITreeViewSolutionFolderRendererType
{
    [Parameter, EditorRequired]
    public DotNetSolutionFolder DotNetSolutionFolder { get; set; } = null!;
}