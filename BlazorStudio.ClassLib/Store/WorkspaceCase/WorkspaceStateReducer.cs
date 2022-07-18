using Fluxor;

namespace BlazorStudio.ClassLib.Store.WorkspaceCase;

public class WorkspaceStateReducer
{
    [ReducerMethod]
    private static WorkspaceState ReduceSetWorkspaceAction(WorkspaceState previousWorkspaceState,
        SetWorkspaceAction setWorkspaceAction)
    {
        return new WorkspaceState(setWorkspaceAction.WorkspaceAbsoluteFilePath);
    }
}