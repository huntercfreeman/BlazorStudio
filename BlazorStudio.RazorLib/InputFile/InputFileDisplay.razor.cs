using System.Collections.Immutable;
using BlazorALaCarte.Shared.Dimensions;
using BlazorALaCarte.Shared.Resize;
using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.BaseTypes;
using BlazorALaCarte.TreeView.Events;
using BlazorALaCarte.TreeView.Services;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileDisplay : FluxorComponent, IInputFileRendererType
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private IState<InputFileState> InputFileStateWrap { get; set; } = null!;

    /// <summary>
    /// Receives the <see cref="_selectedAbsoluteFilePath"/> as
    /// a parameter to the <see cref="RenderFragment"/>
    /// </summary>
    [Parameter]
    public RenderFragment<IAbsoluteFilePath?>? HeaderRenderFragment { get; set; }
    /// <summary>
    /// Receives the <see cref="_selectedAbsoluteFilePath"/> as
    /// a parameter to the <see cref="RenderFragment"/>
    /// </summary>
    [Parameter]
    public RenderFragment<IAbsoluteFilePath?>? FooterRenderFragment { get; set; }
    /// <summary>
    /// One would likely use <see cref="BodyClassCssString"/> in the case where
    /// either <see cref="HeaderRenderFragment"/> or <see cref="FooterRenderFragment"/>
    /// are being used.
    /// <br/><br/>
    /// This is due to one likely wanting to set a fixed height for the <see cref="HeaderRenderFragment"/>
    /// and a fixed height for the <see cref="FooterRenderFragment"/> and lastly
    /// the body gets a fixed height of calc(100% - HeightForHeaderRenderFragment - HeightForFooterRenderFragment); 
    /// </summary>
    [Parameter]
    public string BodyClassCssString { get; set; } = null!;
    /// <summary>
    /// One would likely use <see cref="BodyStyleCssString"/> in the case where
    /// either <see cref="HeaderRenderFragment"/> or <see cref="FooterRenderFragment"/>
    /// are being used.
    /// <br/><br/>
    /// This is due to one likely wanting to set a fixed height for the <see cref="HeaderRenderFragment"/>
    /// and a fixed height for the <see cref="FooterRenderFragment"/> and lastly
    /// the body gets a fixed height of calc(100% - HeightForHeaderRenderFragment - HeightForFooterRenderFragment); 
    /// </summary>
    [Parameter]
    public string BodyStyleCssString { get; set; } = null!;
    
    private readonly ElementDimensions _navMenuElementDimensions = new();
    private readonly ElementDimensions _contentElementDimensions = new();
    
    private IAbsoluteFilePath? _selectedAbsoluteFilePath;
    private TreeViewMouseEventRegistrar _treeViewMouseEventRegistrar = null!;
    private InputFileTreeViewKeymap _inputFileTreeViewKeymap = null!;
    private InputFileTopNavBar? _inputFileTopNavBarComponent;

    /// <summary>
    /// <see cref="_searchMatchTuples"/> feels a bit hacky.
    /// It is currently being used to track what TreeView nodes are both
    /// displayed on the UI and part of the user's search result.
    ///
    /// A presumption that any mutations to the HashSet are done
    /// via the UI thread. Therefore concurrency is not an issue.
    /// </summary>
    private List<(TreeViewStateKey treeViewStateKey, TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)> _searchMatchTuples = new();

    public ElementReference? SearchElementReference => _inputFileTopNavBarComponent?.SearchElementReference;
    
    protected override void OnInitialized()
    {
        _treeViewMouseEventRegistrar = new TreeViewMouseEventRegistrar
        {
            OnClick = TreeViewOnClick,
            OnDoubleClick = TreeViewOnDoubleClick
        };

        _inputFileTreeViewKeymap = new InputFileTreeViewKeymap(
            InputFileContent.TreeViewInputFileContentStateKey,
            TreeViewService,
            InputFileStateWrap,
            Dispatcher,
            CommonComponentRenderers,
            SetInputFileContentTreeViewRoot, () => Task.FromResult(SearchElementReference?.FocusAsync()),
            () => _searchMatchTuples);
        
        InitializeElementDimensions();
        
        base.OnInitialized();
    }

    private void InitializeElementDimensions()
    {
        var navMenuWidth = _navMenuElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);
        
        navMenuWidth.DimensionUnits.AddRange(new []
        {
            new DimensionUnit
            {
                Value = 40,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableColumn.RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        var contentWidth = _contentElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);
        
        contentWidth.DimensionUnits.AddRange(new []
        {
            new DimensionUnit
            {
                Value = 60,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableColumn.RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });
    }
    
    private void SetInputFileContentTreeViewRoot(IAbsoluteFilePath absoluteFilePath)
    {
        var pseudoRootNode = new TreeViewAbsoluteFilePath(
            absoluteFilePath,
            CommonComponentRenderers,
            true,
            false);

        pseudoRootNode.LoadChildrenAsync().Wait();
        
        var adhocRootNode = TreeViewAdhoc.ConstructTreeViewAdhoc(
            pseudoRootNode.Children.ToArray());

        foreach (var child in adhocRootNode.Children)
        {
            child.IsExpandable = false;
        }

        var activeNode = adhocRootNode.Children.FirstOrDefault();
        
        if (!TreeViewService.TryGetTreeViewState(
                InputFileContent.TreeViewInputFileContentStateKey, 
                out var treeViewState))
        {
            TreeViewService.RegisterTreeViewState(new TreeViewState(
                InputFileContent.TreeViewInputFileContentStateKey,
                adhocRootNode,
                activeNode,
                ImmutableList<TreeViewNoType>.Empty));
        }
        else
        {
            TreeViewService.SetRoot(
                InputFileContent.TreeViewInputFileContentStateKey,
                adhocRootNode);
            
            TreeViewService.SetActiveNode(
                InputFileContent.TreeViewInputFileContentStateKey,
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
        if (treeViewMouseEventParameter.TargetNode 
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
        if (treeViewMouseEventParameter.TargetNode 
            is not TreeViewAbsoluteFilePath treeViewAbsoluteFilePath)
        {
            return Task.CompletedTask;
        }

        if (treeViewAbsoluteFilePath.Item != null) 
            SetInputFileContentTreeViewRoot(treeViewAbsoluteFilePath.Item);

        return Task.CompletedTask;
    }
}