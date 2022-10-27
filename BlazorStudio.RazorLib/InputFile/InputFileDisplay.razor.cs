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

public partial class InputFileDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    /// <summary>
    /// Receives the <see cref="_rootTreeViewModel"/> as
    /// as parameter to the <see cref="RenderFragment"/>
    /// </summary>
    [Parameter]
    public RenderFragment<TreeViewModel<IAbsoluteFilePath>?>? HeaderRenderFragment { get; set; }
    /// <summary>
    /// Receives the <see cref="_rootTreeViewModel"/> as
    /// as parameter to the <see cref="RenderFragment"/>
    /// </summary>
    [Parameter]
    public RenderFragment<TreeViewModel<IAbsoluteFilePath>?>? FooterRenderFragment { get; set; }
    /// <summary>
    /// One would likely use <see cref="BodyClassCssString"/> in the case where
    /// either <see cref="HeaderRenderFragment"/> or <see cref="FooterRenderFragment"/>
    /// are being used.
    /// <br/><br/>
    /// This is due to one likely wanting to set a fixed height for the <see cref="HeaderRenderFragment"/>
    /// and a fixed height for the <see cref="FooterRenderFragment"/> and lastly
    /// the body gets a fixed height of calc(100% - HeightForHeaderRenderFragment - HeightForFooterRenderFragment); 
    /// </summary>
    [Parameter]
    public string BodyClassCssString { get; set; } = null!;
    /// <summary>
    /// One would likely use <see cref="BodyStyleCssString"/> in the case where
    /// either <see cref="HeaderRenderFragment"/> or <see cref="FooterRenderFragment"/>
    /// are being used.
    /// <br/><br/>
    /// This is due to one likely wanting to set a fixed height for the <see cref="HeaderRenderFragment"/>
    /// and a fixed height for the <see cref="FooterRenderFragment"/> and lastly
    /// the body gets a fixed height of calc(100% - HeightForHeaderRenderFragment - HeightForFooterRenderFragment); 
    /// </summary>
    [Parameter]
    public string BodyStyleCssString { get; set; } = null!;
    
    private TreeViewModel<IAbsoluteFilePath>? _rootTreeViewModel;
    private ElementDimensions _navMenuElementDimensions = new();
    private ElementDimensions _contentElementDimensions = new();
    
    protected override void OnInitialized()
    {
        var homeAbsoluteFilePath = new AbsoluteFilePath(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            true);
        
        _rootTreeViewModel = new TreeViewModel<IAbsoluteFilePath>(
            homeAbsoluteFilePath, 
            LoadChildrenAsync);

        _rootTreeViewModel.LoadChildrenFuncAsync.Invoke(_rootTreeViewModel);

        InitializeElementDimensions();
        
        base.OnInitialized();
    }

    private void InitializeElementDimensions()
    {
        var navMenuWidth = _navMenuElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);
        
        navMenuWidth.DimensionUnits.AddRange(new []
        {
            new DimensionUnit
            {
                Value = 30,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = 2.5,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        var contentWidth = _contentElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);
        
        contentWidth.DimensionUnits.AddRange(new []
        {
            new DimensionUnit
            {
                Value = 70,
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
}