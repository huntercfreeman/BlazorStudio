﻿using BlazorStudio.ClassLib.Git;
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

        var gitFileParentAbsoluteFilePathString = gitFile.AbsoluteFilePath.ParentDirectory?
            .GetAbsoluteFilePathString();  
            
        var gitFolderParentAbsoluteFilePathString = gitState.GitFolderAbsoluteFilePath.ParentDirectory?
            .GetAbsoluteFilePathString() ?? string.Empty;

        if (gitFileParentAbsoluteFilePathString == gitFolderParentAbsoluteFilePathString)
        {
            return gitFile.AbsoluteFilePath
                .GetAbsoluteFilePathString()
                .Replace(gitFileParentAbsoluteFilePathString, string.Empty);
        }

        return gitFile.AbsoluteFilePath
            .GetAbsoluteFilePathString();
    }
}

