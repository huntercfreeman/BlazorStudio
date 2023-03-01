using System.Collections.Immutable;
using BlazorALaCarte.Shared.Dimensions;
using BlazorALaCarte.Shared.Dropdown;
using BlazorALaCarte.Shared.Icons;
using BlazorALaCarte.Shared.Menu;
using BlazorALaCarte.Shared.Store.ApplicationOptions;
using BlazorALaCarte.Shared.Store.DropdownCase;
using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.BaseTypes;
using BlazorALaCarte.TreeView.Commands;
using BlazorALaCarte.TreeView.Events;
using BlazorALaCarte.TreeView.Services;
using BlazorCommon.RazorLib.Store.ApplicationOptions;
using BlazorCommon.RazorLib.Store.DropdownCase;
using BlazorCommon.RazorLib.TreeView;
using BlazorCommon.RazorLib.TreeView.Commands;
using BlazorCommon.RazorLib.TreeView.TreeViewClasses;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using BlazorStudio.ClassLib.TreeViewImplementations.Helper;
using BlazorStudio.RazorLib.TreeViewImplementations;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.RazorLib.SolutionExplorer;

public partial class SolutionExplorerDisplay : FluxorComponent
{
    [Inject]
    private IState<SolutionExplorerState> SolutionExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IState<TerminalSessionsState> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ClassLib.Menu.ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    
    public static readonly TreeViewStateKey TreeViewSolutionExplorerStateKey = 
        TreeViewStateKey.NewTreeViewStateKey();
    
    private string _filePath = string.Empty;
    private ITreeViewCommandParameter? _mostRecentTreeViewCommandParameter;
    private SolutionExplorerTreeViewKeymap _solutionExplorerTreeViewKeymap = null!;
    private SolutionExplorerTreeViewMouseEventHandler _solutionExplorerTreeViewMouseEventHandler = null!;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsStateWrap.Value.Options.IconSizeInPixels.GetValueOrDefault() *
        (2.0/3.0));

    protected override void OnInitialized()
    {
        _solutionExplorerTreeViewKeymap = new SolutionExplorerTreeViewKeymap(
            TerminalSessionsStateWrap,
            CommonMenuOptionsFactory,
            CommonComponentRenderers,
            FileSystemProvider,
            Dispatcher,
            TreeViewService,
            TextEditorService);
        
        _solutionExplorerTreeViewMouseEventHandler = 
            new SolutionExplorerTreeViewMouseEventHandler(
                Dispatcher,
                TextEditorService,
                CommonComponentRenderers,
                FileSystemProvider,
                TreeViewService);
        
        SolutionExplorerStateWrap.StateChanged += SolutionExplorerStateWrapOnStateChanged;
    
        base.OnInitialized();
    }

    private async void SolutionExplorerStateWrapOnStateChanged(
        object? sender,
        EventArgs e)
    {
        if (SolutionExplorerStateWrap.Value.SolutionAbsoluteFilePath is null)
            return;

        var solutionNamespacePath = new NamespacePath(
            string.Empty,
            SolutionExplorerStateWrap.Value.SolutionAbsoluteFilePath);

        var solutionExplorerNode = new TreeViewNamespacePath(
            solutionNamespacePath,
            CommonComponentRenderers,
            SolutionExplorerStateWrap,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            true)
        {
            TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
        };
    
        await solutionExplorerNode.LoadChildrenAsync();

        if (TreeViewService.TryGetTreeViewState(
                TreeViewSolutionExplorerStateKey, 
                out var treeViewState) &&
            treeViewState is not null)
        {
            await treeViewState.RootNode.LoadChildrenAsync();
            
            TreeViewService.ReRenderNode(
                TreeViewSolutionExplorerStateKey,
                treeViewState.RootNode);
            
            TreeViewService.SetRoot(
                TreeViewSolutionExplorerStateKey,
                solutionExplorerNode);
            
            await InvokeAsync(StateHasChanged);
        }
        else
        {
            TreeViewService.RegisterTreeViewState(new TreeViewState(
                TreeViewSolutionExplorerStateKey,
                solutionExplorerNode,
                solutionExplorerNode,
                ImmutableList<TreeViewNoType>.Empty));
        }
    }
    
    private async Task OnTreeViewContextMenuFunc(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _mostRecentTreeViewCommandParameter = treeViewCommandParameter;
        
        Dispatcher.Dispatch(
            new DropdownsState.AddActiveAction(
                SolutionExplorerContextMenu.ContextMenuEventDropdownKey));
        
        await InvokeAsync(StateHasChanged);
    }
    
    protected override void Dispose(bool disposing)
    {
        SolutionExplorerStateWrap.StateChanged -= SolutionExplorerStateWrapOnStateChanged;
        
        base.Dispose(disposing);
    }
}