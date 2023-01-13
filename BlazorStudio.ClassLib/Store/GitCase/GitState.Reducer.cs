using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Git;
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

            var directoryAbsoluteFilePathString = 
                tryFindGitFolderInDirectoryAction.DirectoryAbsoluteFilePath
                    .GetAbsoluteFilePathString();
            
            var childDirectoryAbsoluteFilePathStrings = Directory.GetDirectories(
                directoryAbsoluteFilePathString);

            var gitFolderAbsoluteFilePathString = childDirectoryAbsoluteFilePathStrings.FirstOrDefault(
                x => x.EndsWith(GitFacts.GIT_FOLDER_NAME));
            
            if (gitFolderAbsoluteFilePathString is not null)
            {
                var gitFolderAbsoluteFilePath = new AbsoluteFilePath(
                    gitFolderAbsoluteFilePathString,
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
        
        [ReducerMethod]
        public static GitState ReduceSetGitFilesListAction(
            GitState previousGitState,
            SetGitFilesListAction setGitFilesListAction)
        {
            return previousGitState with
            {
                GitFilesList = setGitFilesListAction.GitFilesList
            };
        }
        
        [ReducerMethod]
        public static GitState ReduceAddGitFilesAction(
            GitState previousGitState,
            AddGitFilesAction addGitFilesAction)
        {
            var nextGitFilesList = previousGitState.GitFilesList
                .AddRange(addGitFilesAction.GitFilesList);
            
            return previousGitState with
            {
                GitFilesList = nextGitFilesList
            };
        }
    }
}

