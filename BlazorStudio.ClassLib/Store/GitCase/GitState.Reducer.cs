using BlazorStudio.ClassLib.FileSystem.Classes;
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
        
        [ReducerMethod]
        public static GitState ReduceTryFindGitFolderInDirectoryAction(
            GitState previousGitState,
            TryFindGitFolderInDirectoryAction tryFindGitFolderInDirectoryAction)
        {
            if (!tryFindGitFolderInDirectoryAction.DirectoryAbsoluteFilePath.IsDirectory)
                return previousGitState;

            var parentDirectoryAbsoluteFilePathString = 
                tryFindGitFolderInDirectoryAction.DirectoryAbsoluteFilePath
                    .GetAbsoluteFilePathString();
            
            var childDirectories = Directory.GetDirectories(
                parentDirectoryAbsoluteFilePathString);

            var gitFolder = childDirectories.FirstOrDefault(
                x => x == GitFacts.GIT_FOLDER_NAME);
            
            if (gitFolder is not null)
            {
                var gitFolderAbsoluteFilePath = new AbsoluteFilePath(
                    parentDirectoryAbsoluteFilePathString + gitFolder,
                    true);
                
                return previousGitState with
                {
                    GitFolderAbsoluteFilePath = gitFolderAbsoluteFilePath,
                    MostRecentTryFindGitFolderInDirectoryAction = tryFindGitFolderInDirectoryAction
                };
            }

            return previousGitState with
            {
                GitFolderAbsoluteFilePath = null,
                MostRecentTryFindGitFolderInDirectoryAction = tryFindGitFolderInDirectoryAction
            };
        }
    }
}

