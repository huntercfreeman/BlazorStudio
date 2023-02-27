using BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.BalcTreeView.WatchWindowExample;

public partial class TreeViewReflectionDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TreeViewReflection TreeViewReflection { get; set; } = null!;

    private string GetCssStylingForValue(Type itemType)
    {
        if (itemType == typeof(string))
        {
            return "bte_string-literal";
        }
        else if (itemType == typeof(bool))
        {
            return "bte_keyword";
        }
        else
        {
            return string.Empty;
        }
    }
}