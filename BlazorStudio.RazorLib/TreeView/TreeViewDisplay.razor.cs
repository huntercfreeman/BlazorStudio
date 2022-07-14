using System.Collections.Immutable;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeView;

public partial class TreeViewDisplay<T>
    where T : class
{
    [CascadingParameter]
    public Func<T, Task<IEnumerable<T>>> GetChildrenFunc { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeView<T> TreeView { get; set; } = null!;

    private async Task GetChildrenAsync()
    {
        TreeView.Children = (await GetChildrenFunc(TreeView.Item))
            .Select(x => (ITreeView) new TreeView<T>(TreeViewKey.NewTreeViewKey(), x)) 
            .ToArray();

        await InvokeAsync(StateHasChanged);
    }
}