using System.Collections.Immutable;
using BlazorALaCarte.Shared.Dimensions;
using BlazorALaCarte.Shared.Menu;
using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.TreeViewCase;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileContent : FluxorComponent
{
    [Inject]
    private IState<InputFileState> InputFileStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private BlazorStudio.ClassLib.Menu.ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;

    [CascadingParameter(Name = "SetInputFileContentTreeViewRoot")]
    public Action<IAbsoluteFilePath> SetInputFileContentTreeViewRoot { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsoluteFilePath?> SetSelectedAbsoluteFilePath { get; set; } = null!;
    
    public static readonly TreeViewStateKey TreeViewInputFileContentStateKey = 
        TreeViewStateKey.NewTreeViewStateKey();

    private TreeViewMouseEventRegistrar _treeViewMouseEventRegistrar = null!;
    
    protected override void OnInitialized()
    {
        _treeViewMouseEventRegistrar = new TreeViewMouseEventRegistrar
        {
            OnClick = TreeViewOnClick,
            OnDoubleClick = TreeViewOnDoubleClick
        };

        if (!TreeViewService.TryGetTreeViewState(
                InputFileContent.TreeViewInputFileContentStateKey, 
                out var treeViewState))
        {
            var directoryHomeAbsoluteFilePath = new AbsoluteFilePath(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                true);

            SetInputFileContentTreeViewRoot.Invoke(directoryHomeAbsoluteFilePath);
        }
        
        base.OnInitialized();
    }

    private Task TreeViewOnClick(
        TreeViewMouseEventParameter treeViewMouseEventParameter)
    {
        if (treeViewMouseEventParameter.MouseTargetedTreeView 
            is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)
        {
            return Task.CompletedTask;
        }
        
        var setSelectedTreeViewModelAction = 
            new InputFileState.SetSelectedTreeViewModelAction(
                treeViewAbsoluteFilePath);
        
        Dispatcher.Dispatch(setSelectedTreeViewModelAction);
        
        return Task.CompletedTask;
    }
    
    private Task TreeViewOnDoubleClick(
        TreeViewMouseEventParameter treeViewMouseEventParameter)
    {
        if (treeViewMouseEventParameter.MouseTargetedTreeView 
                is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)
        {
            return Task.CompletedTask;
        }

        if (treeViewAbsoluteFilePath.Item != null) 
            SetInputFileContentTreeViewRoot.Invoke(treeViewAbsoluteFilePath.Item);

        return Task.CompletedTask;
    }
}