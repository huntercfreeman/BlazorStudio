using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Git;
using BlazorStudio.ClassLib.Store.GitCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Git.InternalComponents;

public partial class GitFileDisplay : ComponentBase
{
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;
    [Inject]
    private IState<SolutionExplorerState> SolutionExplorerStateWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public GitFile GitFile { get; set; } = null!;

    private string TryShortenGitFilePath(
        IAbsoluteFilePath absoluteFilePath,
        IAbsoluteFilePath shortenByStartsWithAbsoluteFilePath)
    {
        var shortenByStartsWithAbsoluteFilePathString = shortenByStartsWithAbsoluteFilePath.ParentDirectory?
            .GetAbsoluteFilePathString() ?? string.Empty;

        var absoluteFilePathString = absoluteFilePath
            .GetAbsoluteFilePathString();
        
        if (absoluteFilePathString.StartsWith(shortenByStartsWithAbsoluteFilePathString))
        {
            return new string (absoluteFilePathString
                .Skip(shortenByStartsWithAbsoluteFilePathString.Length)
                .ToArray());
        }

        return absoluteFilePathString;
    }
}

