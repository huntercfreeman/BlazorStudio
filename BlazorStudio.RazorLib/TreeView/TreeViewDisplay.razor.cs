using System.Collections.Immutable;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeView;

public partial class TreeViewDisplay<T>
    where T : class
{
    [Inject]
    private IStateSelection<TreeViewWrapStates, ITreeViewWrap?> TreeViewWrapStateSelection { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public Func<T, Task<IEnumerable<T>>> GetChildrenFunc { get; set; } = null!;
    [CascadingParameter]
    public RenderFragment<T> ChildContent { get; set; } = null!;
    /// <summary>
    /// Depth starts at 0
    /// </summary>
    [CascadingParameter(Name="Depth")]
    public int Depth { get; set; }
    [CascadingParameter]
    public TreeViewWrapKey TreeViewWrapKey { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeView<T> TreeView { get; set; } = null!;

    private const int DEPTH_PADDING_LEFT_SCALING_IN_PIXELS = 12;

    /// <summary>
    /// This is used to ensure Children are only loaded once
    /// when the user expands and collapses many times in a row.
    ///
    /// A separate way to force refresh children needs to exist
    /// </summary>
    private bool _shouldLoadChildren = true;

    protected override void OnInitialized()
    {
        TreeViewWrapStateSelection.Select(x =>
        {
            x.Map.TryGetValue(TreeViewWrapKey, out var value);

            return value;
        });

        base.OnInitialized();
    }

    private string GetIsActiveStyling => TreeViewWrapStateSelection.Value?.ActiveTreeViews
        .Select(x => x.Key)
        .Contains(TreeView.Key) ?? false
            ? "bstudio_active"
            : string.Empty;

    private string GetScaledByDepthPixelsOffset(int depth) => $"{depth * DEPTH_PADDING_LEFT_SCALING_IN_PIXELS}px";

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
    
    private void SetIsActiveOnClick()
    {
        Dispatcher.Dispatch(new SetActiveTreeViewAction(TreeViewWrapKey, TreeView));
    }
}