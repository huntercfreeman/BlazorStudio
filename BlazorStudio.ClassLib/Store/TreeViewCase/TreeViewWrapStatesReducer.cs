using BlazorStudio.ClassLib.Sequence;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public class TreeViewWrapStatesReducer
{
    [ReducerMethod]
    public static TreeViewWrapStates ReduceRegisterTreeViewWrapAction(TreeViewWrapStates previousTreeViewWrapStates,
        RegisterTreeViewWrapAction registerTreeViewWrapAction)
    {
        var nextMap = previousTreeViewWrapStates.Map
            .Add(registerTreeViewWrapAction.TreeViewWrap.Key,
                registerTreeViewWrapAction.TreeViewWrap);

        return new TreeViewWrapStates(nextMap);
    }

    [ReducerMethod]
    public static TreeViewWrapStates ReduceDisposeTreeViewWrapAction(TreeViewWrapStates previousTreeViewWrapStates,
        DisposeTreeViewWrapAction disposeTreeViewWrapAction)
    {
        var nextMap = previousTreeViewWrapStates.Map
            .Remove(disposeTreeViewWrapAction.TreeViewWrapKey);

        return new TreeViewWrapStates(nextMap);
    }
    
    [ReducerMethod]
    public static TreeViewWrapStates ReduceSetActiveTreeViewAction(TreeViewWrapStates previousTreeViewWrapStates,
        SetActiveTreeViewAction setActiveTreeViewAction)
    {
        var treeViewWrap = previousTreeViewWrapStates.Map[setActiveTreeViewAction.TreeViewWrapKey];

        // TODO: Allow for more than one active ITreeView
        treeViewWrap.ActiveTreeViews.Clear();
        treeViewWrap.ActiveTreeViews.Add(setActiveTreeViewAction.TreeView);

        treeViewWrap.SequenceKey = SequenceKey.NewSequenceKey();

        return previousTreeViewWrapStates;
    }
}