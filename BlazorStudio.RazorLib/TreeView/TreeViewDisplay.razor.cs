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
    [CascadingParameter]
    public RenderFragment<T> ChildContent { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeView<T> TreeView { get; set; } = null!;

    /// <summary>
    /// This is used to ensure Children are only loaded once
    /// when the user expands and collapses many times in a row.
    ///
    /// A separate way to force refresh children needs to exist
    /// </summary>
    private bool _shouldLoadChildren = true;
    
    private async Task GetChildrenAsync()
    {
        TreeView.Children = (await GetChildrenFunc(TreeView.Item))
            .Select(x => (ITreeView) new TreeView<T>(TreeViewKey.NewTreeViewKey(), x)) 
            .ToArray();

        _shouldLoadChildren = false;

        await InvokeAsync(StateHasChanged);
    }
    
    private void ToggleIsExpandedOnClick()
    {
        if (_shouldLoadChildren)
        {
            _ = Task.Run(async () => await GetChildrenAsync());
        }

        TreeView.IsExpanded = !TreeView.IsExpanded;
    }
}