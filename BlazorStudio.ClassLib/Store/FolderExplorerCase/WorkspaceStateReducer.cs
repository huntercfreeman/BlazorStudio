using Fluxor;

namespace BlazorStudio.ClassLib.Store.FolderExplorerCase;

public class WorkspaceStateReducer
{
    [ReducerMethod]
    public static FolderExplorerState ReduceSetWorkspaceAction(FolderExplorerState previousFolderExplorerState,
        SetFolderExplorerAction setFolderExplorerAction)
    {
        return new FolderExplorerState(setFolderExplorerAction.FolderAbsoluteFilePath);
    }
}