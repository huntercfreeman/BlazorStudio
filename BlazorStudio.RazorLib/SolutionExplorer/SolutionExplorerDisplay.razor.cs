using System.Collections.Immutable;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.CustomEvents;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.ClassLib.Store.TextEditorResourceMapCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using BlazorStudio.ClassLib.TreeViewImplementations.Helper;
using BlazorStudio.RazorLib.TreeViewImplementations;
using BlazorTextEditor.RazorLib;
using BlazorTreeView.RazorLib;
using BlazorTreeView.RazorLib.Keymap;
using BlazorTreeView.RazorLib.Store.TreeViewCase;
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
    private IState<TextEditorResourceMapState> TextEditorResourceMapStateWrap { get; set; } = null!;
    [Inject]
    private IState<TerminalSessionsState> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions SolutionExplorerElementDimensions { get; set; } = null!;

    public static readonly TreeViewStateKey TreeViewSolutionExplorerStateKey = 
        TreeViewStateKey.NewTreeViewStateKey();
    
    private string _filePath = string.Empty;
    private TreeViewContextMenuEvent? _mostRecentTreeViewContextMenuEvent;
    private SolutionExplorerTreeViewKeymap _solutionExplorerTreeViewKeymap = null!;
    private TreeViewMouseEventRegistrar _treeViewMouseEventRegistrar = null!;

    protected override void OnInitialized()
    {
        _solutionExplorerTreeViewKeymap = new SolutionExplorerTreeViewKeymap(
            TerminalSessionsStateWrap,
            CommonMenuOptionsFactory,
            CommonComponentRenderers,
            Dispatcher,
            TreeViewService,
            TextEditorService,
            TextEditorResourceMapStateWrap);
        
        _treeViewMouseEventRegistrar = new TreeViewMouseEventRegistrar
        {
            OnDoubleClick = TreeViewOnDoubleClick
        };
        
        SolutionExplorerStateWrap.StateChanged += SolutionExplorerStateWrapOnStateChanged;
    
        base.OnInitialized();
    }

    private void SolutionExplorerStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (SolutionExplorerStateWrap.Value.SolutionAbsoluteFilePath is null)
            return;

        var solutionNamespacePath = new NamespacePath(
            string.Empty,
            SolutionExplorerStateWrap.Value.SolutionAbsoluteFilePath);

        var solutionExplorerNode = new TreeViewNamespacePath(
            solutionNamespacePath,
            CommonComponentRenderers,
            SolutionExplorerStateWrap)
        {
            IsExpandable = true,
            IsExpanded = false,
            TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
        };

        if (TreeViewService.TryGetTreeViewState(
                TreeViewSolutionExplorerStateKey, out var treeViewState) &&
            treeViewState is not null)
        {
            TreeViewService.SetRoot(
                TreeViewSolutionExplorerStateKey, 
                solutionExplorerNode);
            
            TreeViewService.SetActiveNode(
                TreeViewSolutionExplorerStateKey, 
                solutionExplorerNode);
            
            solutionExplorerNode.LoadChildrenAsync().Wait();
                        
            TreeViewService.ReRenderNode(
                TreeViewSolutionExplorerStateKey,
                solutionExplorerNode);
            
            treeViewState.RootNode.LoadChildrenAsync().Wait();
                        
            TreeViewService.ReRenderNode(
                TreeViewSolutionExplorerStateKey,
                treeViewState.RootNode);

            InvokeAsync(StateHasChanged);
        }
        else
        {
            TreeViewService.RegisterTreeViewState(new TreeViewState(
                TreeViewSolutionExplorerStateKey,
                solutionExplorerNode,
                solutionExplorerNode));
        }
    }
    
    private async Task OnTreeViewContextMenuFunc(TreeViewContextMenuEvent treeViewContextMenuEvent)
    {
        _mostRecentTreeViewContextMenuEvent = treeViewContextMenuEvent;
        
        Dispatcher.Dispatch(
            new AddActiveDropdownKeyAction(
                SolutionExplorerContextMenu.ContextMenuEventDropdownKey));
        
        await InvokeAsync(StateHasChanged);
    }

    private async Task TreeViewOnDoubleClick(
        TreeViewMouseEventParameter treeViewMouseEventParameter)
    {
        if (treeViewMouseEventParameter.MouseTargetedTreeView 
            is not TreeViewNamespacePath treeViewNamespacePath)
        {
            return;
        }

        if (treeViewNamespacePath.Item is null)
            return;

        await EditorState.OpenInEditorAsync(
            treeViewNamespacePath.Item.AbsoluteFilePath,
            Dispatcher,
            TextEditorService,
            TextEditorResourceMapStateWrap.Value);
    }
    
    protected override void Dispose(bool disposing)
    {
        SolutionExplorerStateWrap.StateChanged -= SolutionExplorerStateWrapOnStateChanged;
        
        base.Dispose(disposing);
    }
}