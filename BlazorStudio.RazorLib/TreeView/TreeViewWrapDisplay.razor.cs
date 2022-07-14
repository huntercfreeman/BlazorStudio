using System.Collections.Immutable;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeView;

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

    protected override void OnInitialized()
    {
        TreeViewWrapStateSelection.Select(x =>
        {
            x.Map.TryGetValue(TreeViewWrapKey, out var value);

            return value;
        });

        if (TreeViewWrapStateSelection.Value is null)
        {
            var iterableRootItems = RootItems.ToImmutableArray();

            // TreeViewWrap is uninitialized
            Dispatcher.Dispatch(new RegisterTreeViewWrapRecordAction(new TreeViewWrap<T>(TreeViewWrapKey)
            {
                RootTreeViewRecords = iterableRootItems.
                    Select(x => (ITreeViewRecord)
                        new TreeViewRecord<T>(TreeViewKey.NewTreeViewKey()))
                    .ToImmutableList()
            }));
        }

        base.OnInitialized();
    }

    protected override void Dispose(bool disposing)
    {
        if (ShouldDispose)
        {
            Dispatcher.Dispatch(new DisposeTreeViewWrapRecordAction(TreeViewWrapKey));
        }

        base.Dispose(disposing);
    }
}