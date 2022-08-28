using Fluxor;

namespace BlazorStudio.ClassLib.Store.SolutionCase;

public class SolutionStateReducer
{
    [ReducerMethod]
    public static SolutionState ReduceSetSolutionAction(SolutionState previousSolutionState,
        SetSolutionStateAction setSolutionStateAction)
    {
        return new(setSolutionStateAction.Solution,
            setSolutionStateAction.ProjectIdToProjectMap,
            setSolutionStateAction.FileAbsoluteFilePathToDocumentMap,
            setSolutionStateAction.FileAbsoluteFilePathToAdditionalDocumentMap);
    }
}