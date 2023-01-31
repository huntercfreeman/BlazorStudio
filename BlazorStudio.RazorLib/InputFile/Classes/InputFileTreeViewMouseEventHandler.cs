using BlazorALaCarte.TreeView.Commands;
using BlazorALaCarte.TreeView.Events;
using BlazorALaCarte.TreeView.Services;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using Fluxor;

namespace BlazorStudio.RazorLib.InputFile.Classes;

public class InputFileTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly IDispatcher _dispatcher;
    private readonly Action<IAbsoluteFilePath> _setInputFileContentTreeViewRoot;

    public InputFileTreeViewMouseEventHandler(
        ITreeViewService treeViewService,
        IDispatcher dispatcher,
        Action<IAbsoluteFilePath> setInputFileContentTreeViewRoot) 
        : base(treeViewService)
    {
        _dispatcher = dispatcher;
        _setInputFileContentTreeViewRoot = setInputFileContentTreeViewRoot;
    }

    public override Task<bool> OnClickAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _ = base.OnClickAsync(treeViewCommandParameter);
        
        if (treeViewCommandParameter.TargetNode 
            is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)
        {
            return Task.FromResult(false);
        }
        
        var setSelectedTreeViewModelAction = 
            new InputFileState.SetSelectedTreeViewModelAction(
                treeViewAbsoluteFilePath);
        
        _dispatcher.Dispatch(setSelectedTreeViewModelAction);
        
        return Task.FromResult(true);
    }

    public override Task<bool> OnDoubleClickAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _ = base.OnDoubleClickAsync(treeViewCommandParameter);

        if (treeViewCommandParameter.TargetNode 
            is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)
        {
            return Task.FromResult(false);
        }

        if (treeViewAbsoluteFilePath.Item != null) 
            _setInputFileContentTreeViewRoot.Invoke(treeViewAbsoluteFilePath.Item);

        return Task.FromResult(true);
    }
}