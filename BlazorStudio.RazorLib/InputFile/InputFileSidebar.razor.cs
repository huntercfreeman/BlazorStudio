using System.Collections.Immutable;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.Store.TreeViewCase;
using BlazorTextEditor.RazorLib.TextEditor;
using BlazorTextEditor.RazorLib.TreeView;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileSidebar : FluxorComponent
{
    [Inject]
    private IState<InputFileState> InputFileStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsoluteFilePath?> SetSelectedAbsoluteFilePath { get; set; } = null!;
    
    private static readonly TreeViewStateKey TreeViewInputFileSidebarStateKey = 
        TreeViewStateKey.NewTreeViewStateKey();

    protected override void OnInitialized()
    {
        var directoryHomeAbsoluteFilePath = new AbsoluteFilePath(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            true);
        
        var directoryHomeNode = new TreeViewAbsoluteFilePath(
            directoryHomeAbsoluteFilePath,
            CommonComponentRenderers)
        {
            IsExpandable = true,
            IsExpanded = false
        };
        
        var directoryRootAbsoluteFilePath = new AbsoluteFilePath(
            "/",
            true);

        var directoryRootNode = new TreeViewAbsoluteFilePath(
            directoryRootAbsoluteFilePath,
            CommonComponentRenderers)
        {
            IsExpandable = true,
            IsExpanded = false
        };
        
        var adhocRootNode = TreeViewAdhoc.ConstructTreeViewAdhoc(
            directoryHomeNode,
            directoryRootNode);
        
        if (!TreeViewService.TryGetTreeViewState(
                TreeViewInputFileSidebarStateKey, out var treeViewState))
        {
            TreeViewService.RegisterTreeViewState(new TreeViewState(
                TreeViewInputFileSidebarStateKey,
                adhocRootNode,
                directoryHomeNode));
        }
        
        base.OnInitialized();
    }
}