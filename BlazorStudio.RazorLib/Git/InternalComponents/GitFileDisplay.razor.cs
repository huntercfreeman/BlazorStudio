using BlazorStudio.ClassLib.Git;
using BlazorStudio.ClassLib.Store.GitCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Git.InternalComponents;

public partial class GitFileDisplay : ComponentBase
{
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public GitFile GitFile { get; set; } = null!;

    private string TryShortenGitFilePath()
    {
        var gitState = GitStateWrap.Value;
        var gitFile = GitFile;

        if (gitState.GitFolderAbsoluteFilePath is null)
        {
            return gitFile.AbsoluteFilePath.ParentDirectory?
                .GetAbsoluteFilePathString()
                   ?? string.Empty;
        }

        var gitFolderParentAbsoluteFilePathString = gitState.GitFolderAbsoluteFilePath.ParentDirectory?
            .GetAbsoluteFilePathString() ?? string.Empty;

        var gitFileAbsoluteFilePathString = gitFile.AbsoluteFilePath
            .GetAbsoluteFilePathString();
        
        if (gitFileAbsoluteFilePathString.StartsWith(gitFolderParentAbsoluteFilePathString))
        {
            return new string (gitFileAbsoluteFilePathString
                .Skip(gitFolderParentAbsoluteFilePathString.Length)
                .ToArray());
        }

        return gitFile.AbsoluteFilePath
            .GetAbsoluteFilePathString();
    }
}

