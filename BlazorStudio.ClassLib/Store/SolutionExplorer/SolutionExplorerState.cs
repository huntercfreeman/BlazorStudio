using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.SolutionExplorer;

[FeatureState]
public record SolutionExplorerState(IAbsoluteFilePath? AbsoluteFilePath)
{
    public SolutionExplorerState() : this(default(IAbsoluteFilePath))
    {
        
    }
    
    public record SetSolutionExplorerStateAction(IAbsoluteFilePath? AbsoluteFilePath);

    private class SolutionExplorerStateReducer
    {
        [ReducerMethod]
        public SolutionExplorerState ReduceSetSolutionExplorerStateAction(SolutionExplorerState previousSolutionExplorerState,
            SetSolutionExplorerStateAction setSolutionExplorerStateAction)
        {
            return previousSolutionExplorerState with
            {
                AbsoluteFilePath = setSolutionExplorerStateAction.AbsoluteFilePath
            };
        }
    }
}