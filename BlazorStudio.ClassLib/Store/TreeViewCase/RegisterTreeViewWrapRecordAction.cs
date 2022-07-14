using Fluxor;

namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public record RegisterTreeViewWrapRecordAction(ITreeViewWrap TreeViewWrap);

public class TreeViewWrapStatesReducer
{
    [ReducerMethod]
    public static TreeViewWrapStates ReduceRegisterTreeViewWrapRecordAction(TreeViewWrapStates previousTreeViewWrapStates,
        RegisterTreeViewWrapRecordAction registerTreeViewWrapRecordAction)
    {
        var nextMap = previousTreeViewWrapStates.Map
            .Add(registerTreeViewWrapRecordAction.TreeViewWrap.Key,
                registerTreeViewWrapRecordAction.TreeViewWrap);

        return new TreeViewWrapStates(nextMap);
    }

    [ReducerMethod]
    public static TreeViewWrapStates ReduceDisposeTreeViewWrapRecordAction(TreeViewWrapStates previousTreeViewWrapStates,
        DisposeTreeViewWrapRecordAction disposeTreeViewWrapRecordAction)
    {
        var nextMap = previousTreeViewWrapStates.Map
            .Remove(disposeTreeViewWrapRecordAction.TreeViewWrapKey);

        return new TreeViewWrapStates(nextMap);
    }
}