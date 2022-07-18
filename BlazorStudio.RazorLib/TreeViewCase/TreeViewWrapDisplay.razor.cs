using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeViewCase;

public partial class TreeViewWrapDisplay<T> : FluxorComponent, IDisposable
    where T : class
{
    [Inject]
    private IStateSelection<TreeViewWrapStates, ITreeViewWrap?> TreeViewWrapStateSelection { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeViewWrapKey TreeViewWrapKey { get; set; } = null!;
    [Parameter, EditorRequired]
    public IEnumerable<T> RootItems { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool ShouldDispose { get; set; }
    [Parameter, EditorRequired]
    public Func<T, Task<IEnumerable<T>>> GetChildrenFunc { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment<T> ChildContent { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<T> OnEnterKeyDown { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<T> OnSpaceKeyDown { get; set; } = null!;

    protected override void OnInitialized()
    {
        TreeViewWrapStateSelection.Select(x =>
        {
            x.Map.TryGetValue(TreeViewWrapKey, out var value);

            return value;
        });

        if (TreeViewWrapStateSelection.Value is null)
        {
            var iterableRootItems = RootItems.ToArray();

            var treeViews = iterableRootItems.Select(x => (ITreeView)
                    new TreeView<T>(TreeViewKey.NewTreeViewKey(), x))
                .ToList();

            List<ITreeView> activeTreeViews;

            if (treeViews.Any())
            {
                activeTreeViews = new List<ITreeView>
                {
                    treeViews[0]
                };
            }
            else
            {
                activeTreeViews = new List<ITreeView>();
            }

            // TreeViewWrap is uninitialized
            Dispatcher.Dispatch(new RegisterTreeViewWrapAction(new TreeViewWrap<T>(TreeViewWrapKey)
            {
                RootTreeViews = treeViews,
                ActiveTreeViews = activeTreeViews
            }));
        }

        base.OnInitialized();
    }

    protected override void Dispose(bool disposing)
    {
        if (ShouldDispose)
        {
            Dispatcher.Dispatch(new DisposeTreeViewWrapAction(TreeViewWrapKey));
        }

        base.Dispose(disposing);
    }
}