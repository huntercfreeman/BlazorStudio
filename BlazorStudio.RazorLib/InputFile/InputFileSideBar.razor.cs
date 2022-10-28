using System.Collections.Immutable;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.TreeView;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileSideBar : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsoluteFilePath>? SetSelectedAbsoluteFilePath { get; set; } = null!;
    
    private TreeViewModel<IAbsoluteFilePath>? _fileSystemTreeViewModel;

    protected override void OnInitialized()
    {
        _fileSystemTreeViewModel = new TreeViewModel<IAbsoluteFilePath>(
            new AbsoluteFilePath(string.Empty, true), 
            LoadTreeViewRoot);

        _fileSystemTreeViewModel.LoadChildrenFuncAsync.Invoke(_fileSystemTreeViewModel);
        
        base.OnInitialized();
    }
    
    private Task LoadTreeViewRoot(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        // HOME
        var homeAbsoluteFilePath = new AbsoluteFilePath(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            true);
        
        var homeTreeViewModel = new TreeViewModel<IAbsoluteFilePath>(
            homeAbsoluteFilePath, 
            LoadChildrenAsync);

        homeTreeViewModel.LoadChildrenFuncAsync.Invoke(homeTreeViewModel);    
            
        // ROOT
        var rootAbsoluteFilePath = new AbsoluteFilePath(
            "/",
            true);
        
        var rootTreeViewModel = new TreeViewModel<IAbsoluteFilePath>(
            rootAbsoluteFilePath, 
            LoadChildrenAsync);

        rootTreeViewModel.LoadChildrenFuncAsync.Invoke(rootTreeViewModel);
        
        treeViewModel.Children.Clear();
        
        treeViewModel.Children.AddRange(new []
        {
            homeTreeViewModel,
            rootTreeViewModel
        });

        return Task.CompletedTask;
    }

    private Task LoadChildrenAsync(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        var absoluteFilePathString = treeViewModel.Item.GetAbsoluteFilePathString();

        var childFiles = Directory
            .GetFiles(absoluteFilePathString)
            .OrderBy(filename => filename)
            .Select(cf => new AbsoluteFilePath(cf, false));
        
        var childDirectories = Directory
            .GetDirectories(absoluteFilePathString)
            .OrderBy(filename => filename)
            .Select(cd => new AbsoluteFilePath(cd, true));

        var childTreeViewModels = childDirectories
            .Union(childFiles)
            .Select(afp => new TreeViewModel<IAbsoluteFilePath>(afp, LoadChildrenAsync));

        treeViewModel.Children.Clear();
        treeViewModel.Children.AddRange(childTreeViewModels);
        
        return Task.CompletedTask;
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
}