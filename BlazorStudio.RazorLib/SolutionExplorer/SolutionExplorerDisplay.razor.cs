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
    private bool _settingsSolutionExplorerState;

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
    
    private Task LoadChildrenAsync(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        return Task.CompletedTask;
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
    
    private Task OpenSolutionOnClick()
    {
        Dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                afp =>
                {
                    _settingsSolutionExplorerState = true;
                    InvokeAsync(StateHasChanged);
                    
                    // Without Task.Run blocks UI thread I believe
                    Task.Run(() =>
                    {
                        Dispatcher.Dispatch(
                            new SolutionExplorerState.RequestSetSolutionExplorerStateAction(
                                afp));
                        
                        _settingsSolutionExplorerState = false;
                        InvokeAsync(StateHasChanged);
                    });
                    
                    return Task.CompletedTask;
                },
                afp =>
                {
                    if (afp is null ||
                        afp.ExtensionNoPeriod != ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
                    {
                        return Task.FromResult(false);
                    }
                    
                    return Task.FromResult(true);
                },
                new []
                {
                    new InputFilePattern(
                        ".NET Solution",
                        afp => 
                            afp.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
                }.ToImmutableArray()));
        
        return Task.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        SolutionExplorerStateWrap.StateChanged -= SolutionExplorerStateWrapOnStateChanged;
        
        base.Dispose(disposing);
    }
}