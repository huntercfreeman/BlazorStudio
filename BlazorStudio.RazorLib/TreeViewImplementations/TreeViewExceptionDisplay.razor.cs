using BlazorStudio.ClassLib.TreeViewImplementations;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeViewImplementations;

public partial class TreeViewExceptionDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewException TreeViewException { get; set; } = null!;
}