using Fluxor;

namespace BlazorStudio.ClassLib.Store.GitCase;

public partial record GitState
{
    private class Reducer
    {
        [ReducerMethod]
        public static GitState ReduceSetGitFolderAbsoluteFilePathAction(
            GitState previousGitState,
            SetGitFolderAbsoluteFilePathAction setGitFolderAbsoluteFilePathAction)
        {
            return previousGitState with
            {
                GitFolderAbsoluteFilePath = setGitFolderAbsoluteFilePathAction.GitFolderAbsoluteFilePath
            };
        }
    }
}

