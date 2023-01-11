using BlazorALaCarte.Shared.Keyboard;
using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.Commands;
using BlazorALaCarte.TreeView.Keymap;
using BlazorALaCarte.TreeView.Services;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile;

public class InputFileTreeViewKeymap : ITreeViewKeymap
{
    private readonly TreeViewStateKey _treeViewStateKey;
    private readonly ITreeViewService _treeViewService;
    private readonly IState<InputFileState> _inputFileStateWrap;
    private readonly IDispatcher _dispatcher;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly Action<IAbsoluteFilePath> _setInputFileContentTreeViewRoot;
    private readonly Func<Task> _focusSearchInputElementFunc;
    private readonly Func<List<(TreeViewStateKey treeViewStateKey, TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)>> _getSearchMatchTuplesFunc;

    public InputFileTreeViewKeymap(TreeViewStateKey treeViewStateKey,
        ITreeViewService treeViewService,
        IState<InputFileState> inputFileStateWrap,
        IDispatcher dispatcher,
        ICommonComponentRenderers commonComponentRenderers,
        Action<IAbsoluteFilePath> setInputFileContentTreeViewRoot,
        Func<Task> focusSearchInputElementFunc,
        Func<List<(TreeViewStateKey treeViewStateKey, TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)>> getSearchMatchTuplesFunc)
    {
        _treeViewStateKey = treeViewStateKey;
        _treeViewService = treeViewService;
        _inputFileStateWrap = inputFileStateWrap;
        _dispatcher = dispatcher;
        _commonComponentRenderers = commonComponentRenderers;
        _setInputFileContentTreeViewRoot = setInputFileContentTreeViewRoot;
        _focusSearchInputElementFunc = focusSearchInputElementFunc;
        _getSearchMatchTuplesFunc = getSearchMatchTuplesFunc;
    }
    
    public bool TryMapKey(
        KeyboardEventArgs keyboardEventArgs, 
        out TreeViewCommand? treeViewCommand)
    {
        switch (keyboardEventArgs.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
                treeViewCommand = new TreeViewCommand(SetInputFileContentTreeViewRoot);
                return true;
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                treeViewCommand = new TreeViewCommand(SetSelectedTreeViewModel);
                return true;
        }

        switch (keyboardEventArgs.Key)
        {
            // Tried to have { "Ctrl" + "f" } => MoveFocusToSearchBar
            // however, the webview was ending up taking over
            // and displaying its search bar with focus being set to it.
            //
            // Doing preventDefault just for this one case would be a can of
            // worms as JSInterop is needed, as well a custom Blazor event.
            case "/":
            case "?":
                treeViewCommand = new TreeViewCommand(MoveFocusToSearchBar);
                return true;
            // TODO: Add move to next match and move to previous match
            //
            // case "*":
            //     treeViewCommand = new TreeViewCommand(SetNextMatchAsActiveTreeViewNode);
            //     return true;
            // case "#":
            //     treeViewCommand = new TreeViewCommand(SetPreviousMatchAsActiveTreeViewNode);
            //     return true;
        }

        if (keyboardEventArgs.AltKey)
            return AltModifiedKeymap(keyboardEventArgs, out treeViewCommand);
        
        treeViewCommand = null;
        return false;
    }
    
    private bool AltModifiedKeymap(
        KeyboardEventArgs keyboardEventArgs,
        out TreeViewCommand? treeViewCommand)
    {
        TreeViewCommand? command = null;
        
        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                command = new TreeViewCommand(HandleBackButtonOnClick);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                command = new TreeViewCommand(HandleUpwardButtonOnClick);
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
                command = new TreeViewCommand(HandleForwardButtonOnClick);
                break;
            case "r":
                command = new TreeViewCommand(HandleRefreshButtonOnClick);
                break;
        }

        treeViewCommand = command;
        
        if (treeViewCommand is null)
            return false;
        
        return true;
    }
    
    private Task SetInputFileContentTreeViewRoot(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        var treeViewAbsoluteFilePath = activeNode as TreeViewAbsoluteFilePath;

        if (treeViewAbsoluteFilePath?.Item is null)
            return Task.CompletedTask;
        
        _setInputFileContentTreeViewRoot.Invoke(treeViewAbsoluteFilePath.Item);
        return Task.CompletedTask;
    }
    
    private Task HandleBackButtonOnClick(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _dispatcher.Dispatch(new InputFileState.MoveBackwardsInHistoryAction());

        ChangeContentRootToOpenedTreeView(_inputFileStateWrap.Value);
        
        return Task.CompletedTask;
    }
    
    private Task HandleForwardButtonOnClick(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _dispatcher.Dispatch(new InputFileState.MoveForwardsInHistoryAction());
        
        ChangeContentRootToOpenedTreeView(_inputFileStateWrap.Value);
        
        return Task.CompletedTask;
    }

    private Task HandleUpwardButtonOnClick(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _dispatcher.Dispatch(new InputFileState.OpenParentDirectoryAction(
            _commonComponentRenderers));
        
        ChangeContentRootToOpenedTreeView(_inputFileStateWrap.Value);

        return Task.CompletedTask;
    }

    private Task HandleRefreshButtonOnClick(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _dispatcher.Dispatch(new InputFileState.RefreshCurrentSelectionAction());
        
        ChangeContentRootToOpenedTreeView(_inputFileStateWrap.Value);
        
        return Task.CompletedTask;
    }
    
    private TreeViewAbsoluteFilePath GetOpenedTreeView(InputFileState inputFileState)
    {
        return inputFileState.OpenedTreeViewModelHistory[
            inputFileState.IndexInHistory];
    }
    
    private void ChangeContentRootToOpenedTreeView(
        InputFileState inputFileState)
    {
        var openedTreeView = GetOpenedTreeView(_inputFileStateWrap.Value);
        
        if (openedTreeView.Item is not null)
            _setInputFileContentTreeViewRoot.Invoke(openedTreeView.Item);
    }
    
    private Task SetSelectedTreeViewModel(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;
        
        var treeViewAbsoluteFilePath = activeNode as TreeViewAbsoluteFilePath;
        
        if (treeViewAbsoluteFilePath is null)
            return Task.CompletedTask;
        
        var setSelectedTreeViewModelAction = 
            new InputFileState.SetSelectedTreeViewModelAction(
                treeViewAbsoluteFilePath);
        
        _dispatcher.Dispatch(setSelectedTreeViewModelAction);
        
        return Task.CompletedTask;
    }
    
    private async Task MoveFocusToSearchBar(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        await _focusSearchInputElementFunc.Invoke();
    }
}