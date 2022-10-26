using Fluxor;

namespace BlazorStudio.ClassLib.Store.RoslynWorkspaceState;

public class RoslynWorkspaceStateReducer
{
    [ReducerMethod]
    public static RoslynWorkspaceState ReduceSetRoslynWorkspaceStateAction(
        RoslynWorkspaceState previousRoslynWorkspaceState,
        SetRoslynWorkspaceStateAction setRoslynWorkspaceStateAction)
    {
        return new RoslynWorkspaceState(setRoslynWorkspaceStateAction.MSBuildWorkspace,
            setRoslynWorkspaceStateAction.VisualStudioInstance,
            setRoslynWorkspaceStateAction.MsBuildAbsoluteFilePath);
    }
}