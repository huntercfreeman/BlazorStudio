using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.TreeViewImplementations;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeViewImplementations;

public partial class TreeViewAbsoluteFilePathDisplay 
    : ComponentBase, ITreeViewAbsoluteFilePathRendererType
{
    [Parameter, EditorRequired]
    public TreeViewAbsoluteFilePath TreeViewAbsoluteFilePath { get; set; } = null!;
}