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

public partial class InputFileNavMenu : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions NavMenuElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<TreeViewModel<IAbsoluteFilePath>?> SetSelectedTreeViewModel { get; set; } = null!;
    
    private ElementDimensions _homeElementDimensions = new();
    private ElementDimensions _rootElementDimensions = new();
    private TreeViewModel<IAbsoluteFilePath>? _homeTreeViewModel;
    private TreeViewModel<IAbsoluteFilePath>? _rootTreeViewModel;

    protected override Task OnInitializedAsync()
    {
        // HOME
        var homeAbsoluteFilePath = new AbsoluteFilePath(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            true);
        
        _homeTreeViewModel = new TreeViewModel<IAbsoluteFilePath>(
            homeAbsoluteFilePath, 
            LoadChildrenAsync);

        _homeTreeViewModel.LoadChildrenFuncAsync.Invoke(_homeTreeViewModel);    
            
        // ROOT
        var rootAbsoluteFilePath = new AbsoluteFilePath(
            "/",
            true);
        
        _rootTreeViewModel = new TreeViewModel<IAbsoluteFilePath>(
            rootAbsoluteFilePath, 
            LoadChildrenAsync);

        _rootTreeViewModel.LoadChildrenFuncAsync.Invoke(_rootTreeViewModel);

        InvokeAsync(StateHasChanged);
        
        return base.OnInitializedAsync();
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
            "Open in Text Editor",
            () => OpenInTextEditor(treeViewModel));

        return new MenuRecord(new []
        {
            openInTextEditorMenuOption
        }.ToImmutableArray());
    }

    private void OpenInTextEditor(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        _ = Task.Run(async () =>
        {
            var fileContents = await File
                .ReadAllTextAsync(treeViewModel.Item.GetAbsoluteFilePathString());

            var textEditor = new TextEditorBase(
                fileContents,
                null,
                null,
                null);
            
            TextEditorService.RegisterTextEditor(textEditor);
        });
    }

    private string GetStyleForContextMenu(MouseEventArgs? mouseEventArgs)
    {
        if (mouseEventArgs is null)
            return string.Empty;

        return 
            $"position: fixed; left: {mouseEventArgs.ClientX}px; top: {mouseEventArgs.ClientY}px;";
    }
    
    private void InitializeElementDimensions()
    {
        var homeTreeHeight = _homeElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Height);
        
        homeTreeHeight.DimensionUnits.AddRange(new []
        {
            new DimensionUnit
            {
                Value = 50,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = 2.5,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        var rootTreeHeight = _rootElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Height);
        
        rootTreeHeight.DimensionUnits.AddRange(new []
        {
            new DimensionUnit
            {
                Value = 50,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = 2.5,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });
    }
}