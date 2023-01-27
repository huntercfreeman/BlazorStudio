using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.Store.GitCase;

public partial record GitState
{
    public record SetGitStateWithAction(Func<GitState, GitState> GitStateWithFunc);
    
    public record TryFindGitFolderInDirectoryAction(
        IAbsoluteFilePath DirectoryAbsoluteFilePath,
        CancellationToken CancellationToken);
    
    public record RefreshGitAction(CancellationToken CancellationToken);
    public record GitInitAction(CancellationToken CancellationToken);
}