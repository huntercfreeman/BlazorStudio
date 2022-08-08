using System.Collections.Immutable;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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
    public RenderFragment<T> ItemRenderFragment { get; set; } = null!;
    /// <summary>
    /// Action parameters: Action(T item, Action toggleIsExpanded)
    /// </summary>
    [Parameter, EditorRequired]
    public Action<T, Action> OnEnterKeyDown { get; set; } = null!;
    /// <summary>
    /// Action parameters: Action(T item, Action toggleIsExpanded)
    /// </summary>
    [Parameter, EditorRequired]
    public Action<T, Action> OnSpaceKeyDown { get; set; } = null!;
    /// <summary>
    /// Action parameters: Action(T item, Action toggleIsExpanded, MouseEventArgs mouseEventArgs)
    /// </summary>
    [Parameter, EditorRequired]
    public Action<T, Action, MouseEventArgs>? OnDoubleClick { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<T, bool> IsExpandable { get; set; } = null!;
    /// <summary>
    /// If <see cref="OnContextMenu"/> is provided then:
    /// upon ContextMenuEvent event the Action will be invoked.
    /// 
    /// If ContextMenu event occurs with { 'F10' + 'ShiftKey' }
    /// the MouseEventArgs will be null.
    /// </summary>
    [Parameter]
    public Action<T, MouseEventArgs?>? OnContextMenu { get; set; }
    /// <summary>
    /// If <see cref="OnContextMenuRenderFragment"/> is provided then:
    /// upon ContextMenuEvent event the RenderFragment will be rendered.
    ///
    /// If ContextMenu event occurs with { 'F10' + 'ShiftKey' }
    /// the MouseEventArgs will be null.
    /// </summary>
    [Parameter]
    public RenderFragment<ContextMenuEventDto<T>>? OnContextMenuRenderFragment { get; set; }
    [Parameter]
    public RenderFragment<ImmutableArray<T>>? FooterRenderFragment { get; set; }
    [Parameter]
    public RenderFragment<ImmutableArray<T>>? HeaderRenderFragment { get; set; }
    [Parameter]
    public string BodyCssClassAttribute { get; set; } = string.Empty;
    [Parameter]
    public string StyleString { get; set; } = string.Empty;

    private int _sequence;

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

    public void Reload()
    {
        Dispatcher.Dispatch(new DisposeTreeViewWrapAction(TreeViewWrapKey));

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

    protected override void Dispose(bool disposing)
    {
        if (ShouldDispose)
        {
            Dispatcher.Dispatch(new DisposeTreeViewWrapAction(TreeViewWrapKey));
        }

        base.Dispose(disposing);
    }

    public class ContextMenuEventDto<T>
    {
        public T Item { get; set; }
        public MouseEventArgs? MouseEventArgs { get; set; }
        public Func<Task> RefreshContextMenuTarget { get; set; }
        public Func<Task> RefreshParentOfContextMenuTarget { get; set; }
    }
}