using BlazorCommon.RazorLib.TreeView.TreeViewClasses;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.GitCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TreeViewImplementations;

public partial class TreeViewGitFileDisplay 
    : FluxorComponent, ITreeViewGitFileRendererType
{
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;
    
    [CascadingParameter]
    public TreeViewState TreeViewState { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TreeViewGitFile TreeViewGitFile { get; set; } = null!;

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