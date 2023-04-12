using BlazorCommon.RazorLib.BackgroundTaskCase;
using BlazorCommon.RazorLib.TreeView;
using BlazorCommon.RazorLib.TreeView.Commands;
using BlazorCommon.RazorLib.TreeView.Events;
using BlazorStudio.ClassLib.ComponentRenderers;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using BlazorTextEditor.RazorLib;
using Fluxor;

namespace BlazorStudio.RazorLib.FolderExplorer.Classes;

public class FolderExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly IDispatcher _dispatcher;
    private readonly ITextEditorService _textEditorService;
    private readonly IBlazorStudioComponentRenderers _blazorStudioComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;

    public FolderExplorerTreeViewMouseEventHandler(
        IDispatcher dispatcher,
        ITextEditorService textEditorService,
        IBlazorStudioComponentRenderers blazorStudioComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        ITreeViewService treeViewService,
        IBackgroundTaskQueue backgroundTaskQueue) 
        : base(treeViewService)
    {
        _dispatcher = dispatcher;
        _textEditorService = textEditorService;
        _blazorStudioComponentRenderers = blazorStudioComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
        _backgroundTaskQueue = backgroundTaskQueue;
    }
    
    public override async Task<bool> OnDoubleClickAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _ = base.OnDoubleClickAsync(treeViewCommandParameter);

        if (treeViewCommandParameter.TargetNode 
            is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)
        {
            return false;
        }

        if (treeViewAbsoluteFilePath.Item is null)
            return false;

        await EditorState.OpenInEditorAsync(
            treeViewAbsoluteFilePath.Item,
            true,
            _dispatcher,
            _textEditorService,
            _blazorStudioComponentRenderers,
            _fileSystemProvider,
            _backgroundTaskQueue);
        
        return true;
    }
}