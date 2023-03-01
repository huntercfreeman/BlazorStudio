using BlazorCommon.RazorLib.TreeView;
using BlazorCommon.RazorLib.TreeView.Commands;
using BlazorCommon.RazorLib.TreeView.Events;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using BlazorTextEditor.RazorLib;
using Fluxor;

namespace BlazorStudio.RazorLib.SolutionExplorer;

public class SolutionExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly IDispatcher _dispatcher;
    private readonly ITextEditorService _textEditorService;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;

    public SolutionExplorerTreeViewMouseEventHandler(
        IDispatcher dispatcher,
        ITextEditorService textEditorService,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        ITreeViewService treeViewService) 
        : base(treeViewService)
    {
        _dispatcher = dispatcher;
        _textEditorService = textEditorService;
        _commonComponentRenderers = commonComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
    }
    
    public override async Task<bool> OnDoubleClickAsync(
        ITreeViewCommandParameter treeViewCommandParameter)
    {
        _ = base.OnDoubleClickAsync(treeViewCommandParameter);

        if (treeViewCommandParameter.TargetNode 
            is not TreeViewNamespacePath treeViewNamespacePath)
        {
            return false;
        }

        if (treeViewNamespacePath.Item is null)
            return false;

        await EditorState.OpenInEditorAsync(
            treeViewNamespacePath.Item.AbsoluteFilePath,
            _dispatcher,
            _textEditorService,
            _commonComponentRenderers,
            _fileSystemProvider);
        
        return true;
    }
}