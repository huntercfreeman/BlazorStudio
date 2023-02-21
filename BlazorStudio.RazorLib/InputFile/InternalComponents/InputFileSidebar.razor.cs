using System.Collections.Immutable;
using BlazorALaCarte.DialogNotification.Dialog;
using BlazorALaCarte.Shared.Dimensions;
using BlazorALaCarte.Shared.Store.DropdownCase;
using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.BaseTypes;
using BlazorALaCarte.TreeView.Commands;
using BlazorALaCarte.TreeView.Services;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using BlazorStudio.RazorLib.InputFile.Classes;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.InputFile.InternalComponents;

public partial class InputFileSidebar : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    
    [CascadingParameter(Name = "SetInputFileContentTreeViewRootFunc")]
    public Func<IAbsoluteFilePath, Task> SetInputFileContentTreeViewRootFunc { get; set; } = null!;
    [CascadingParameter]
    public InputFileTreeViewMouseEventHandler InputFileTreeViewMouseEventHandler { get; set; } = null!;
    [CascadingParameter]
    public InputFileTreeViewKeyboardEventHandler InputFileTreeViewKeyboardEventHandler { get; set; } = null!;
    [CascadingParameter]
    public InputFileState InputFileState { get; set; } = null!;
    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsoluteFilePath?> SetSelectedAbsoluteFilePath { get; set; } = null!;
    
    public static readonly TreeViewStateKey TreeViewInputFileSidebarStateKey = 
        TreeViewStateKey.NewTreeViewStateKey();
    
    private ITreeViewCommandParameter? _mostRecentTreeViewCommandParameter;

    protected override void OnInitialized()
    {
        var directoryHomeNode = new TreeViewAbsoluteFilePath(
            EnvironmentProvider.HomeDirectoryAbsoluteFilePath,
            CommonComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            false);
    
        var directoryRootNode = new TreeViewAbsoluteFilePath(
            EnvironmentProvider.RootDirectoryAbsoluteFilePath,
            CommonComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            false);
        
        var adhocRootNode = TreeViewAdhoc.ConstructTreeViewAdhoc(
            directoryHomeNode,
            directoryRootNode);
        
        if (!TreeViewService.TryGetTreeViewState(
                TreeViewInputFileSidebarStateKey, out var treeViewState))
        {
            TreeViewService.RegisterTreeViewState(new TreeViewState(
                TreeViewInputFileSidebarStateKey,
                adhocRootNode,
                directoryHomeNode,
                ImmutableList<TreeViewNoType>.Empty));
        }
        
        base.OnInitialized();
    }
    
    private async Task OnTreeViewContextMenuFunc(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _mostRecentTreeViewCommandParameter = treeViewCommandParameter;
        
        Dispatcher.Dispatch(
            new DropdownsState.AddActiveAction(
                InputFileContextMenu.ContextMenuEventDropdownKey));
        
        await InvokeAsync(StateHasChanged);
    }
}