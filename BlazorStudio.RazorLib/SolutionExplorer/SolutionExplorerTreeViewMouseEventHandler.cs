using BlazorALaCarte.TreeView.Commands;
using BlazorALaCarte.TreeView.Events;
using BlazorALaCarte.TreeView.Services;
using BlazorStudio.ClassLib.CommonComponents;
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

    public SolutionExplorerTreeViewMouseEventHandler(
        IDispatcher dispatcher,
        ITextEditorService textEditorService,
        ICommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService) 
        : base(treeViewService)
    {
        _dispatcher = dispatcher;
        _textEditorService = textEditorService;
        _commonComponentRenderers = commonComponentRenderers;
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
            _commonComponentRenderers);
        
        return true;
    }
}