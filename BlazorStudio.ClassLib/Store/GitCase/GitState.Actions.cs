using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Git;

namespace BlazorStudio.ClassLib.Store.GitCase;

public partial record GitState
{
    public record SetGitFolderAbsoluteFilePathAction(IAbsoluteFilePath? GitFolderAbsoluteFilePath);
    public record TryFindGitFolderInDirectoryAction(IAbsoluteFilePath DirectoryAbsoluteFilePath);
    public record SetGitFilesListAction(ImmutableList<GitFile> GitFilesList);
}