using System.Collections.Immutable;
using BlazorStudio.ClassLib.CustomEvents;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.TreeView;
using BlazorStudio.RazorLib.TreeView;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileContent : FluxorComponent
{
    [Inject]
    private IState<InputFileState> InputFileStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsoluteFilePath?> SetSelectedAbsoluteFilePath { get; set; } = null!;

    private TreeViewDisplayOnEventRegistration<IAbsoluteFilePath> _treeViewDisplayOnEventRegistration = null!;
    
    private TreeViewModel<IAbsoluteFilePath> SelectionMutablyReferenced => 
        InputFileStateWrap.Value.OpenedTreeViewModelHistory[
            InputFileStateWrap.Value.IndexInHistory];

    protected override void OnInitialized()
    {
        _treeViewDisplayOnEventRegistration = new TreeViewDisplayOnEventRegistration<IAbsoluteFilePath>();
        
        _treeViewDisplayOnEventRegistration.AfterClickFuncAsync = AfterClickFuncAsync; 
        _treeViewDisplayOnEventRegistration.AfterDoubleClickFuncAsync = AfterDoubleClickFuncAsync; 
        _treeViewDisplayOnEventRegistration.AfterKeyDownFuncAsync = AfterKeyDownFuncAsync; 
        
        base.OnInitialized();
    }

    private Task AfterClickFuncAsync(
        MouseEventArgs mouseEventArgs, 
        TreeViewDisplay<IAbsoluteFilePath> treeViewDisplay)
    {
        Dispatcher.Dispatch(
            new InputFileState.SetSelectedTreeViewModelAction(
                treeViewDisplay.TreeViewModel));

        return Task.CompletedTask;
    }
    
    private Task AfterDoubleClickFuncAsync(
        MouseEventArgs mouseEventArgs, 
        TreeViewDisplay<IAbsoluteFilePath> treeViewDisplay)
    {
        Dispatcher.Dispatch(
            new InputFileState.SetOpenedTreeViewModelAction(
                treeViewDisplay.TreeViewModel));

        return Task.CompletedTask;
    }
    
    private Task AfterKeyDownFuncAsync(
        BsKeyDownEventArgs bsKeyDownEventArgs, 
        TreeViewDisplay<IAbsoluteFilePath> treeViewDisplay)
    {
        if (bsKeyDownEventArgs.AltWasPressed)
        {
            switch (bsKeyDownEventArgs.Key)
            {
                case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_LEFT:
                    Dispatcher.Dispatch(
                        new InputFileState.MoveBackwardsInHistoryAction());
                    break;
                case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN:
                    Dispatcher.Dispatch(
                        new InputFileState.SetOpenedTreeViewModelAction(
                            treeViewDisplay.TreeViewModel));
                    break;
                case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_UP:
                    Dispatcher.Dispatch(
                        new InputFileState.OpenParentDirectoryAction());
                    break;
                case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_RIGHT:
                    Dispatcher.Dispatch(
                        new InputFileState.MoveForwardsInHistoryAction());
                    break;
            }
        }
        else
        {
            switch (bsKeyDownEventArgs.Code)
            {
                case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
                    Dispatcher.Dispatch(
                        new InputFileState.SetOpenedTreeViewModelAction(
                            treeViewDisplay.TreeViewModel));
                    break;
                case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                    Dispatcher.Dispatch(
                        new InputFileState.SetSelectedTreeViewModelAction(
                            treeViewDisplay.TreeViewModel));
                    break;
            }
        }

        return Task.CompletedTask;
    }

    private MenuRecord GetContextMenu(
        TreeViewDisplayContextMenuEvent<IAbsoluteFilePath> treeViewDisplayContextMenuEvent)
    {
        var treeViewModel = treeViewDisplayContextMenuEvent.TreeViewDisplay.TreeViewModel;
        
        return new MenuRecord(new []
        {
            CommonMenuOptionsFactory.NewEmptyFile(
                treeViewModel.Item,
                async () => {})
        }.ToImmutableArray());
    }
}