using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeView;

public partial class TreeViewDisplay<T>
    where T : class
{
    [Parameter, EditorRequired]
    public T Item { get; set; } = null!;
}