using BlazorALaCarte.TreeView.Events;
using BlazorALaCarte.TreeView.Services;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using Fluxor;

namespace BlazorStudio.RazorLib.InputFile;

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
        TreeViewMouseEventParameter treeViewMouseEventParameter)
    {
        _ = base.OnClickAsync(treeViewMouseEventParameter);
        
        if (treeViewMouseEventParameter.TargetNode 
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
        TreeViewMouseEventParameter treeViewMouseEventParameter)
    {
        _ = base.OnDoubleClickAsync(treeViewMouseEventParameter);

        if (treeViewMouseEventParameter.TargetNode 
            is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)
        {
            return Task.FromResult(false);
        }

        if (treeViewAbsoluteFilePath.Item != null) 
            _setInputFileContentTreeViewRoot.Invoke(treeViewAbsoluteFilePath.Item);

        return Task.FromResult(true);
    }
}