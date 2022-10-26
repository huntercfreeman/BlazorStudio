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
        // The user of the TreeView component might have 'OnClick' close the TreeView causing a not found exception
        if (previousTreeViewWrapStates.Map.TryGetValue(setActiveTreeViewAction.TreeViewWrapKey, out var treeViewWrap))
        {
            // TODO: Allow for more than one active ITreeView
            treeViewWrap.ActiveTreeViews.Clear();
            treeViewWrap.ActiveTreeViews.Add(setActiveTreeViewAction.TreeView);

            var nextTreeViewWrapStatesMap = previousTreeViewWrapStates.Map
                .SetItem(treeViewWrap.Key, treeViewWrap.CloneShallow());

            return new TreeViewWrapStates(nextTreeViewWrapStatesMap);
        }

        return previousTreeViewWrapStates;
    }
    
    [ReducerMethod]
    public static TreeViewWrapStates ReduceAddTreeViewRootsAction(TreeViewWrapStates previousTreeViewWrapStates,
        AddTreeViewRootsAction addTreeViewRootsAction)
    {
        // The user of the TreeView component might have 'OnClick' close the TreeView causing a not found exception
        if (previousTreeViewWrapStates.Map.TryGetValue(addTreeViewRootsAction.TreeViewWrapKey, out var treeViewWrap))
        {
            var iterableRootItems = addTreeViewRootsAction.RootTreeViews.ToArray();

            treeViewWrap.RootTreeViews.AddRange(iterableRootItems);

            return previousTreeViewWrapStates;
        }

        return previousTreeViewWrapStates;
    }
}