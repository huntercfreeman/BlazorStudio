using BlazorStudio.ClassLib.FileSystem.Classes;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.SolutionCase;

public class SolutionStateReducer
{
    [ReducerMethod]
    public static SolutionState ReduceSetSolutionAction(SolutionState previousSolutionState,
        SetSolutionAction setSolutionAction)
    {
        return new(setSolutionAction.Solution,
            setSolutionAction.ProjectIdToProjectMap,
            setSolutionAction.FileAbsoluteFilePathToDocumentMap);
    }
}