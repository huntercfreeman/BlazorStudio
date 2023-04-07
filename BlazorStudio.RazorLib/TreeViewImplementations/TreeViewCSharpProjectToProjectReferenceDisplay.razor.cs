using BlazorStudio.ClassLib.ComponentRenderers.Types;
using BlazorStudio.ClassLib.DotNet.CSharp;
using BlazorStudio.ClassLib.Nuget;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeViewImplementations;

public partial class TreeViewCSharpProjectToProjectReferenceDisplay : 
    ComponentBase, ITreeViewCSharpProjectToProjectReferenceRendererType
{
    [Parameter, EditorRequired]
    public CSharpProjectToProjectReference CSharpProjectToProjectReference { get; set; } = null!;
}