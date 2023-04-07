using BlazorStudio.ClassLib.ComponentRenderers.Types;
using BlazorStudio.ClassLib.Nuget;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeViewImplementations;

public partial class TreeViewLightWeightNugetPackageRecordDisplay : 
    ComponentBase, ITreeViewLightWeightNugetPackageRecordRendererType
{
    [Parameter, EditorRequired]
    public LightWeightNugetPackageRecord LightWeightNugetPackageRecord { get; set; } = null!;
}