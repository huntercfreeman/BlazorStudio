using Fluxor;

namespace BlazorStudio.ClassLib.Store.SolutionCase;

public class SolutionReducer
{
    [ReducerMethod]
    public static SolutionState ReduceSetSolutionWorkspaceAction(SolutionState previousSolutionState,
        SetSolutionAction setSolutionAction)
    {
        return new(setSolutionAction.SolutionWorkspace);
    }
}