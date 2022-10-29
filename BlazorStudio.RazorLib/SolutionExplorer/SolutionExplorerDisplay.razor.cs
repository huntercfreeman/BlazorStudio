using System.Collections.Immutable;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorStudio.ClassLib.TreeView;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.SolutionExplorer;

public partial class SolutionExplorerDisplay : FluxorComponent
{
    [Inject]
    private IState<SolutionExplorerState> SolutionExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions SolutionExplorerElementDimensions { get; set; } = null!;

    private string _filePath = string.Empty;
    private TreeViewModel<IAbsoluteFilePath>? _solutionTreeViewModel;

    protected override void OnInitialized()
    {
        SolutionExplorerStateWrap.StateChanged += SolutionExplorerStateWrapOnStateChanged;
        
        base.OnInitialized();
    }

    private void SolutionExplorerStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (SolutionExplorerStateWrap.Value.SolutionAbsoluteFilePath is null)
            return;

        _solutionTreeViewModel = new TreeViewModel<IAbsoluteFilePath>(
            SolutionExplorerStateWrap.Value.SolutionAbsoluteFilePath,
            true,
            LoadChildrenAsync);

        _solutionTreeViewModel.LoadChildrenFuncAsync.Invoke(_solutionTreeViewModel);
    }
    
    private async Task LoadChildrenAsync(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        if (treeViewModel.Item.IsDirectory)
        {
            await LoadChildrenForDirectoryAsync(treeViewModel);
        }
        else
        {
            switch (treeViewModel.Item.ExtensionNoPeriod)
            {
                case ExtensionNoPeriodFacts.DOT_NET_SOLUTION:
                    await LoadChildrenForSolutionAsync(treeViewModel);
                    break;
                case ExtensionNoPeriodFacts.C_SHARP_PROJECT:
                    await LoadChildrenForCSharpProjectAsync(treeViewModel);
                    break;
                default:
                    await LoadNestedChildrenAsync(treeViewModel);
                    break;
            }
        }
    }
    
    private async Task LoadChildrenForSolutionAsync(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        var solutionExplorerState = SolutionExplorerStateWrap.Value;

        if (solutionExplorerState.Solution is null)
            return;

        var childProjects = solutionExplorerState.Solution.Projects
            .Select(x => new TreeViewModel<IAbsoluteFilePath>(
                new AbsoluteFilePath(x.FilePath, false),
                true,
                LoadChildrenAsync));

        treeViewModel.Children.Clear();
        treeViewModel.Children.AddRange(childProjects);
    }
    
    private async Task LoadChildrenForCSharpProjectAsync(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        var parentDirectoryOfCSharpProject = ((IAbsoluteFilePath)treeViewModel.Item.Directories
            .Last());

        var parentAbsoluteFilePathString = parentDirectoryOfCSharpProject
            .GetAbsoluteFilePathString();
        
        var childDirectoryTreeViewModels = Directory
            .GetDirectories(parentAbsoluteFilePathString)
            .Select(x => 
                new TreeViewModel<IAbsoluteFilePath>(
                    new AbsoluteFilePath(x, true),
                    true,
                    LoadChildrenAsync));
        
        var childFileTreeViewModels = Directory
            .GetFiles(parentAbsoluteFilePathString)
            .Select(x => 
                new TreeViewModel<IAbsoluteFilePath>(
                    new AbsoluteFilePath(x, false),
                    false,
                    LoadChildrenAsync));

        var allChildTreeViewModels = childDirectoryTreeViewModels
            .Union(childFileTreeViewModels);

        treeViewModel.Children.Clear();
        treeViewModel.Children.AddRange(allChildTreeViewModels);
    }
    
    private async Task LoadChildrenForDirectoryAsync(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        var directoryAbsoluteFilePathString = treeViewModel.Item
            .GetAbsoluteFilePathString();
        
        var childDirectoryTreeViewModels = Directory
            .GetDirectories(directoryAbsoluteFilePathString)
            .Select(x => 
                new TreeViewModel<IAbsoluteFilePath>(
                    new AbsoluteFilePath(x, true),
                    true,
                    LoadChildrenAsync));
        
        var childFileTreeViewModels = Directory
            .GetFiles(directoryAbsoluteFilePathString)
            .Select(x => 
                new TreeViewModel<IAbsoluteFilePath>(
                    new AbsoluteFilePath(x, false),
                    false,
                    LoadChildrenAsync));

        var allChildTreeViewModels = childDirectoryTreeViewModels
            .Union(childFileTreeViewModels);

        treeViewModel.Children.Clear();
        treeViewModel.Children.AddRange(allChildTreeViewModels);
    }

    /// <summary>
    /// This method is used for .razor and .razor.cs codebehinds being nested
    /// in the solution explorer. As well for any other 'codebehind' relationship.
    /// </summary>
    private async Task LoadNestedChildrenAsync(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
    }
    
    private void DispatchSetFolderExplorerStateOnClick()
    {
        if (!Directory.Exists(_filePath))
            throw new DirectoryNotFoundException();
        
        Dispatcher.Dispatch(
            new SetFolderExplorerStateAction(
                new AbsoluteFilePath(_filePath, true)));
    }
    
    private MenuRecord GetContextMenu(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        var openInTextEditorMenuOption = new MenuOptionRecord(
            "Nothing here TODO: Aaa",
            () => { });

        return new MenuRecord(new []
        {
            openInTextEditorMenuOption
        }.ToImmutableArray());
    }

    private string GetStyleForContextMenu(MouseEventArgs? mouseEventArgs)
    {
        if (mouseEventArgs is null)
            return string.Empty;

        return 
            $"position: fixed; left: {mouseEventArgs.ClientX}px; top: {mouseEventArgs.ClientY}px;";
    }

    protected override void Dispose(bool disposing)
    {
        SolutionExplorerStateWrap.StateChanged -= SolutionExplorerStateWrapOnStateChanged;
        
        base.Dispose(disposing);
    }
}