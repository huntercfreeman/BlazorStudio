using System.Collections.Immutable;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.CustomEvents;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using BlazorTreeView.RazorLib;
using BlazorTreeView.RazorLib.Store.TreeViewCase;
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
    private ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsoluteFilePath?> SetSelectedAbsoluteFilePath { get; set; } = null!;
    
    private static readonly TreeViewStateKey TreeViewInputFileContentStateKey = 
        TreeViewStateKey.NewTreeViewStateKey();

    private TreeViewMouseEventRegistrar _treeViewMouseEventRegistrar = null!;
    
    protected override void OnInitialized()
    {
        _treeViewMouseEventRegistrar = new TreeViewMouseEventRegistrar
        {
            OnClick = TreeViewOnClick,
            OnDoubleClick = TreeViewOnDoubleClick
        };
        
        var directoryHomeAbsoluteFilePath = new AbsoluteFilePath(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            true);

        SetTreeViewRoot(directoryHomeAbsoluteFilePath);
        
        base.OnInitialized();
    }

    private void SetTreeViewRoot(IAbsoluteFilePath absoluteFilePath)
    {
        var pseudoRootNode = new TreeViewAbsoluteFilePath(
            absoluteFilePath,
            CommonComponentRenderers)
        {
            IsExpandable = true,
            IsExpanded = false
        };

        pseudoRootNode.LoadChildrenAsync().Wait();
        
        var adhocRootNode = TreeViewAdhoc.ConstructTreeViewAdhoc(
            pseudoRootNode.Children.ToArray());

        foreach (var child in adhocRootNode.Children)
        {
            child.IsExpandable = false;
        }

        var activeNode = adhocRootNode.Children.FirstOrDefault();
        
        if (!TreeViewService.TryGetTreeViewState(
                TreeViewInputFileContentStateKey, out var treeViewState))
        {
            TreeViewService.RegisterTreeViewState(new TreeViewState(
                TreeViewInputFileContentStateKey,
                adhocRootNode,
                activeNode));
        }
        else
        {
            TreeViewService.SetRoot(
                TreeViewInputFileContentStateKey,
                adhocRootNode);
            
            TreeViewService.SetActiveNode(
                TreeViewInputFileContentStateKey,
                activeNode);
        }

        var setOpenedTreeViewModelAction = new InputFileState.SetOpenedTreeViewModelAction(
            pseudoRootNode,
            CommonComponentRenderers);
        
        Dispatcher.Dispatch(setOpenedTreeViewModelAction);
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
            SetTreeViewRoot(treeViewAbsoluteFilePath.Item);

        return Task.CompletedTask;
    }
}