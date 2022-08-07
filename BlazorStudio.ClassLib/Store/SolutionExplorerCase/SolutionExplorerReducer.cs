using Fluxor;

namespace BlazorStudio.ClassLib.Store.SolutionExplorerCase;

public class SolutionExplorerReducer
{
    [ReducerMethod]
    public static SolutionExplorerState ReduceSetSolutionExplorerAction(SolutionExplorerState previousSolutionExplorerState,
        SetSolutionExplorerAction setSolutionExplorerAction)
    {
        return new SolutionExplorerState(setSolutionExplorerAction.SolutionAbsoluteFilePath, 
            setSolutionExplorerAction.SequenceKey);
    }
}