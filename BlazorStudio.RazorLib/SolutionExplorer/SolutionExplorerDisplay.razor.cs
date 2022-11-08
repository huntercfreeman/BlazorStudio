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
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorStudio.ClassLib.Store.TextEditorResourceMapCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using BlazorStudio.ClassLib.TreeViewImplementations.Helper;
using BlazorStudio.RazorLib.TreeViewImplementations;
using BlazorTextEditor.RazorLib;
using BlazorTreeView.RazorLib;
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
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions SolutionExplorerElementDimensions { get; set; } = null!;

    private static readonly TreeViewStateKey TreeViewSolutionExplorerStateKey = 
        TreeViewStateKey.NewTreeViewStateKey();
    
    private string _filePath = string.Empty;

    private const char NAMESPACE_DELIMITER = '.';
    
    protected override void OnInitialized()
    {
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
        
        TreeViewService.RegisterTreeViewState(new TreeViewState(
            TreeViewSolutionExplorerStateKey,
            solutionExplorerNode,
            solutionExplorerNode));
    }
    
    protected override void Dispose(bool disposing)
    {
        SolutionExplorerStateWrap.StateChanged -= SolutionExplorerStateWrapOnStateChanged;
        
        base.Dispose(disposing);
    }
}