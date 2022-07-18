using System.Collections.Immutable;
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
    public RenderFragment<T> ItemRenderFragment { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<T> OnEnterKeyDown { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<T> OnSpaceKeyDown { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<T, bool> IsExpandable { get; set; } = null!;
    [Parameter]
    public RenderFragment<ImmutableArray<T>>? FooterRenderFragment { get; set; }
    [Parameter]
    public RenderFragment<RenderFragment>? BodyRenderFragment { get; set; }
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

    private RenderFragment GetInnerRenderFragment(ITreeView[] rootTreeViews)
    {
        return builder =>
        {
            for (int i = 0; i < rootTreeViews.Length; i++)
            {
                var child = (TreeView<T>)rootTreeViews[i];

                builder.OpenComponent<TreeViewDisplay<T>>(_sequence++);

                builder.AddAttribute(_sequence++, "TreeView", child);
                builder.AddAttribute(_sequence++, "GetSiblingsAndSelfFunc", () => rootTreeViews);
                builder.AddAttribute(_sequence++, "IndexAmongSiblings", i);
                builder.AddAttribute(_sequence++, "Parent", (T?) null);

                builder.CloseComponent();
            }
        };
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