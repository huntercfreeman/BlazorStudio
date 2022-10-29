using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.WorkspaceCase;
using Fluxor;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.AspNetCore.Components;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace BlazorStudio.ClassLib.Store.SolutionExplorer;

[FeatureState]
public record SolutionExplorerState(
    IAbsoluteFilePath? SolutionAbsoluteFilePath,
    Solution? Solution,
    IAbsoluteFilePath? MsBuildAbsoluteFilePath,
    MSBuildWorkspace? MsBuildWorkspace)
{
    public SolutionExplorerState() : this(
        default, 
        default,
        default,
        default)
    {
        
    }
    
    public record RequestSetSolutionExplorerStateAction(
        IAbsoluteFilePath? SolutionAbsoluteFilePath);

    private class SolutionExplorerStateReducer
    {
        [ReducerMethod]
        public SolutionExplorerState ReduceSetSolutionExplorerStateAction(
            SolutionExplorerState previousSolutionExplorerState,
            SolutionExplorerStateEffects.SetSolutionExplorerStateAction setSolutionExplorerStateAction)
        {
            return previousSolutionExplorerState with
            {
                SolutionAbsoluteFilePath = 
                    setSolutionExplorerStateAction.SolutionAbsoluteFilePath,
                Solution = 
                    setSolutionExplorerStateAction.Solution,
            };
        }
    }
    
    private class SolutionExplorerStateEffects
    {
        private readonly IState<WorkspaceState> _workspaceStateWrap;

        public SolutionExplorerStateEffects(IState<WorkspaceState> workspaceStateWrap)
        {
            _workspaceStateWrap = workspaceStateWrap;
        }
        
        public record SetSolutionExplorerStateAction(
            IAbsoluteFilePath? SolutionAbsoluteFilePath,
            Solution? Solution);

        [EffectMethod]
        public async Task HandleSetSolutionExplorerStateAction(
            RequestSetSolutionExplorerStateAction requestSetSolutionExplorerStateAction,
            IDispatcher dispatcher)
        {
            if (_workspaceStateWrap.Value.Workspace is null)
            {
                dispatcher.Dispatch(new SetWorkspaceStateAction());
            }
            
            var solution = await ((MSBuildWorkspace)_workspaceStateWrap.Value.Workspace)
                .OpenSolutionAsync(requestSetSolutionExplorerStateAction
                    .SolutionAbsoluteFilePath
                    .GetAbsoluteFilePathString());
            
            dispatcher.Dispatch(
                new SetSolutionExplorerStateAction(
                    requestSetSolutionExplorerStateAction
                        .SolutionAbsoluteFilePath,
                    solution));
        }
    }
}