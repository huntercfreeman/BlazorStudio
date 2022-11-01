using Fluxor;

namespace BlazorStudio.ClassLib.Store.FolderExplorerCase;

public class FolderExplorerStateReducer
{
    [ReducerMethod]
    public FolderExplorerState ReduceSetFolderExplorerStateAction(FolderExplorerState previousFolderExplorerState,
        SetFolderExplorerStateAction setFolderExplorerStateAction)
    {
        return previousFolderExplorerState with
        {
            AbsoluteFilePath = setFolderExplorerStateAction.AbsoluteFilePath
        };
    }
}