using System.Collections.Immutable;
using BlazorStudio.ClassLib.CustomEvents;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorStudio.ClassLib.Store.TextEditorResourceMapCase;
using BlazorStudio.ClassLib.TreeView;
using BlazorStudio.RazorLib.TreeView;
using BlazorTextEditor.RazorLib;
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
    private IState<TextEditorResourceMapState> TextEditorResourceMapStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions SolutionExplorerElementDimensions { get; set; } = null!;

    private string _filePath = string.Empty;
    private TreeViewModel<NamespacePath>? _solutionTreeViewModel;
    private TreeViewDisplayOnEventRegistration<NamespacePath> _treeViewDisplayOnEventRegistration = null!;

    private const char NAMESPACE_DELIMITER = '.';
    
    protected override void OnInitialized()
    {
        SolutionExplorerStateWrap.StateChanged += SolutionExplorerStateWrapOnStateChanged;
    
        _treeViewDisplayOnEventRegistration = new TreeViewDisplayOnEventRegistration<NamespacePath>();
        
        _treeViewDisplayOnEventRegistration.AfterClickFuncAsync = AfterClickFuncAsync; 
        _treeViewDisplayOnEventRegistration.AfterDoubleClickFuncAsync = AfterDoubleClickFuncAsync; 
        _treeViewDisplayOnEventRegistration.AfterKeyDownFuncAsync = AfterKeyDownFuncAsync;
        
        base.OnInitialized();
    }

    private void SolutionExplorerStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (SolutionExplorerStateWrap.Value.SolutionAbsoluteFilePath is null)
            return;

        var solutionNamespacePath = new NamespacePath(
            string.Empty,
            SolutionExplorerStateWrap.Value.SolutionAbsoluteFilePath);
        
        _solutionTreeViewModel = new TreeViewModel<NamespacePath>(
            solutionNamespacePath,
            true,
            LoadChildrenAsync);

        _solutionTreeViewModel.LoadChildrenFuncAsync.Invoke(_solutionTreeViewModel);
    }
    
    private async Task LoadChildrenAsync(TreeViewModel<NamespacePath> treeViewModel)
    {
        if (treeViewModel.Item.AbsoluteFilePath.IsDirectory)
        {
            await LoadChildrenForDirectoryAsync(treeViewModel);
        }
        else
        {
            switch (treeViewModel.Item.AbsoluteFilePath.ExtensionNoPeriod)
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

        // There are some context menu options which
        // perform a fire and forget task.
        //
        // When that task finishes it will tell the TreeViewModel to
        // reload its children. But the task does not understand how to
        // rerender the user interface.
        //
        // Having this state has changed allows fire and forget tasks
        // to rerender the user interface.
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task LoadChildrenForSolutionAsync(TreeViewModel<NamespacePath> treeViewModel)
    {
        var solutionExplorerState = SolutionExplorerStateWrap.Value;

        if (solutionExplorerState.Solution is null)
            return;

        var childProjects = solutionExplorerState.Solution.Projects
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x.FilePath, false);
                
                return new TreeViewModel<NamespacePath>(
                    new NamespacePath(
                        absoluteFilePath.FileNameNoExtension,
                        absoluteFilePath),
                    true,
                    LoadChildrenAsync);
            })
            .ToList();
        
        RestorePreviousStates(
            treeViewModel.Children,
            childProjects);

        treeViewModel.Children.Clear();
        treeViewModel.Children.AddRange(childProjects);
    }
    
    private async Task LoadChildrenForCSharpProjectAsync(TreeViewModel<NamespacePath> treeViewModel)
    {
        var parentDirectoryOfCSharpProject = ((IAbsoluteFilePath)treeViewModel
            .Item.AbsoluteFilePath.Directories
            .Last());

        var parentAbsoluteFilePathString = parentDirectoryOfCSharpProject
            .GetAbsoluteFilePathString();
        
        var hiddenFiles = HiddenFileFacts
            .GetHiddenFilesByContainerFileExtension(ExtensionNoPeriodFacts.C_SHARP_PROJECT);
        
        var childDirectoryTreeViewModels = Directory
            .GetDirectories(parentAbsoluteFilePathString)
            .Where(x => hiddenFiles.All(hidden => !x.EndsWith(hidden)))
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, true);

                var namespaceString = treeViewModel.Item.Namespace +
                                      NAMESPACE_DELIMITER +
                                      absoluteFilePath.FileNameNoExtension;

                return new TreeViewModel<NamespacePath>(
                    new NamespacePath(
                        namespaceString,
                        absoluteFilePath),
                    true,
                    LoadChildrenAsync);
            });
        
        var uniqueDirectories = UniqueFileFacts
            .GetUniqueFilesByContainerFileExtension(
                ExtensionNoPeriodFacts.C_SHARP_PROJECT);
        
        var foundUniqueDirectories = new List<TreeViewModel<NamespacePath>>();
        var foundDefaultDirectories = new List<TreeViewModel<NamespacePath>>();

        foreach (var directoryTreeViewModel in childDirectoryTreeViewModels)
        {
            if (uniqueDirectories.Any(unique => directoryTreeViewModel.Item.AbsoluteFilePath.FileNameNoExtension == unique))
                foundUniqueDirectories.Add(directoryTreeViewModel);
            else
                foundDefaultDirectories.Add(directoryTreeViewModel);
        }
        
        foundUniqueDirectories = foundUniqueDirectories
            .OrderBy(x => x.Item.AbsoluteFilePath.FileNameNoExtension)
            .ToList();

        foundDefaultDirectories = foundDefaultDirectories
            .OrderBy(x => x.Item.AbsoluteFilePath.FileNameNoExtension)
            .ToList();
        
        var childFileTreeViewModels = Directory
            .GetFiles(parentAbsoluteFilePathString)
            .Where(x => !x.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, false);

                var namespaceString = treeViewModel.Item.Namespace;
                
                return new TreeViewModel<NamespacePath>(
                    new NamespacePath(
                        namespaceString,
                        absoluteFilePath),
                    false,
                    LoadChildrenAsync);
            });
        
        var allChildTreeViewModels = 
            foundUniqueDirectories
            .Union(foundDefaultDirectories)
            .Union(childFileTreeViewModels)
            .ToList();
        
        RestorePreviousStates(
            treeViewModel.Children,
            allChildTreeViewModels);

        treeViewModel.Children.Clear();
        treeViewModel.Children.AddRange(allChildTreeViewModels);
    }
    
    private async Task LoadChildrenForDirectoryAsync(TreeViewModel<NamespacePath> treeViewModel)
    {
        var directoryAbsoluteFilePathString = treeViewModel.Item.AbsoluteFilePath
            .GetAbsoluteFilePathString();
        
        var childDirectoryTreeViewModels = Directory
            .GetDirectories(directoryAbsoluteFilePathString)
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, true);

                var namespaceString = treeViewModel.Item.Namespace +
                                      NAMESPACE_DELIMITER +
                                      absoluteFilePath.FileNameNoExtension;
                
                return new TreeViewModel<NamespacePath>(
                    new NamespacePath(
                        namespaceString, 
                        absoluteFilePath),
                    true,
                    LoadChildrenAsync);
            });
        
        var childFileTreeViewModels = Directory
            .GetFiles(directoryAbsoluteFilePathString)
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, false);

                var namespaceString = treeViewModel.Item.Namespace;
                
                return new TreeViewModel<NamespacePath>(
                    new NamespacePath(
                        namespaceString,
                        absoluteFilePath),
                    false,
                    LoadChildrenAsync);
            });

        var allChildTreeViewModels = childDirectoryTreeViewModels
            .Union(childFileTreeViewModels)
            .ToList();

        RestorePreviousStates(
            treeViewModel.Children,
            allChildTreeViewModels);

        treeViewModel.Children.Clear();
        treeViewModel.Children.AddRange(allChildTreeViewModels);
    }

    /// <summary>
    /// This method is used for .razor and .razor.cs codebehinds being nested
    /// in the solution explorer. As well for any other 'codebehind' relationship.
    /// </summary>
    private async Task LoadNestedChildrenAsync(TreeViewModel<NamespacePath> treeViewModel)
    {
    }
    
    private void RestorePreviousStates(
        List<TreeViewModel<NamespacePath>> previousChildren,
        List<TreeViewModel<NamespacePath>> nextChildren)
    {
        var previousChildrenIsExpandedMap = previousChildren
            .ToDictionary(
                x => x.Item.AbsoluteFilePath.GetAbsoluteFilePathString(),
                x => x);

        foreach (var nextChild in nextChildren)
        {
            if (previousChildrenIsExpandedMap.TryGetValue(
                    nextChild.Item.AbsoluteFilePath.GetAbsoluteFilePathString(), 
                    out var previousTreeViewModel))
            {
                nextChild.RestoreState(previousTreeViewModel);
            }
        }
    } 
    
    private void DispatchSetFolderExplorerStateOnClick()
    {
        if (!Directory.Exists(_filePath))
            throw new DirectoryNotFoundException();
        
        Dispatcher.Dispatch(
            new SetFolderExplorerStateAction(
                new AbsoluteFilePath(_filePath, true)));
    }
    
    private Task AfterClickFuncAsync(
        MouseEventArgs mouseEventArgs, 
        TreeViewDisplay<NamespacePath> treeViewDisplay)
    {
        return Task.CompletedTask;
    }
    
    private async Task AfterDoubleClickFuncAsync(
        MouseEventArgs mouseEventArgs, 
        TreeViewDisplay<NamespacePath> treeViewDisplay)
    {
        await EditorState.OpenInEditorAsync(
            treeViewDisplay.TreeViewModel.Item.AbsoluteFilePath,
            Dispatcher,
            TextEditorService,
            TextEditorResourceMapStateWrap.Value);
    }
    
    private async Task AfterKeyDownFuncAsync(
        CustomKeyDownEventArgs customKeyDownEventArgs, 
        TreeViewDisplay<NamespacePath> treeViewDisplay)
    {
        switch (customKeyDownEventArgs.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
                await EditorState.OpenInEditorAsync(
                    treeViewDisplay.TreeViewModel.Item.AbsoluteFilePath,
                    Dispatcher,
                    TextEditorService,
                    TextEditorResourceMapStateWrap.Value);
                break;
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                treeViewDisplay.TreeViewModel.IsExpanded = 
                    !treeViewDisplay.TreeViewModel.IsExpanded;
                
                _ = Task.Run(async () =>
                {
                    await treeViewDisplay.TreeViewModel.LoadChildrenFuncAsync
                        .Invoke(treeViewDisplay.TreeViewModel);
                    
                    await InvokeAsync(StateHasChanged);
                });
                
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        SolutionExplorerStateWrap.StateChanged -= SolutionExplorerStateWrapOnStateChanged;
        
        base.Dispose(disposing);
    }
}