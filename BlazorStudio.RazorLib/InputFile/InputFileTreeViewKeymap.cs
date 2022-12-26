using BlazorALaCarte.Shared.Keyboard;
using BlazorALaCarte.TreeView.Commands;
using BlazorALaCarte.TreeView.Keymap;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile;

public class InputFileTreeViewKeymap : ITreeViewKeymap
{
    private readonly IState<InputFileState> _inputFileStateWrap;
    private readonly IDispatcher _dispatcher;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly Action<IAbsoluteFilePath> _setInputFileContentTreeViewRoot;

    public InputFileTreeViewKeymap(
        IState<InputFileState> inputFileStateWrap,
        IDispatcher dispatcher,
        ICommonComponentRenderers commonComponentRenderers,
        Action<IAbsoluteFilePath> setInputFileContentTreeViewRoot)
    {
        _inputFileStateWrap = inputFileStateWrap;
        _dispatcher = dispatcher;
        _commonComponentRenderers = commonComponentRenderers;
        _setInputFileContentTreeViewRoot = setInputFileContentTreeViewRoot;
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
    
    private async Task SetInputFileContentTreeViewRoot(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        var activeNode = treeViewCommandParameter.TreeViewState.ActiveNode;

        var treeViewAbsoluteFilePath = activeNode as TreeViewAbsoluteFilePath;

        if (treeViewAbsoluteFilePath?.Item is null)
            return;
        
        _setInputFileContentTreeViewRoot.Invoke(treeViewAbsoluteFilePath.Item);
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
}