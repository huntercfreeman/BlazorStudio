using Fluxor;

namespace BlazorStudio.ClassLib.Store.WorkspaceCase;

public class WorkspaceStateReducer
{
    [ReducerMethod]
    public static WorkspaceState ReduceSetWorkspaceAction(WorkspaceState previousWorkspaceState,
        SetWorkspaceAction setWorkspaceAction)
    {
        return new WorkspaceState(setWorkspaceAction.WorkspaceAbsoluteFilePath);
    }
}