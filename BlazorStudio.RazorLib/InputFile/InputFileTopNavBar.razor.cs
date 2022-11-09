using System.Collections.Immutable;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileTopNavBar : FluxorComponent
{
    [Inject]
    private IState<InputFileState> InputFileStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;

    [CascadingParameter(Name="SetInputFileContentTreeViewRoot")]
    public Action<IAbsoluteFilePath> SetInputFileContentTreeViewRoot { get; set; } = null!;
    
    private ElementReference? _searchElementReference;
    private string _searchQuery = string.Empty;

    public string SearchQuery
    {
        get => _searchQuery;
        set => Dispatcher
            .Dispatch(
                new InputFileState.SetSearchQueryAction(
                    value));
    }

    protected override void OnInitialized()
    {
        InputFileStateWrap.StateChanged += InputFileStateWrapOnStateChanged;
        
        base.OnInitialized();
    }

    private void InputFileStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        _searchQuery = InputFileStateWrap.Value.SearchQuery;
        InvokeAsync(StateHasChanged);
    }

    private void HandleBackButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.MoveBackwardsInHistoryAction());

        ChangeContentRootToOpenedTreeView(InputFileStateWrap.Value);
    }
    
    private void HandleForwardButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.MoveForwardsInHistoryAction());
        
        ChangeContentRootToOpenedTreeView(InputFileStateWrap.Value);
    }

    private void HandleUpwardButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.OpenParentDirectoryAction(
            CommonComponentRenderers));
        
        ChangeContentRootToOpenedTreeView(InputFileStateWrap.Value);
    }

    private void HandleRefreshButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.RefreshCurrentSelectionAction());
        
        ChangeContentRootToOpenedTreeView(InputFileStateWrap.Value);
    }

    private void FocusSearchElementReferenceOnClick()
    {
        _searchElementReference?.FocusAsync();
    }

    private TreeViewAbsoluteFilePath GetOpenedTreeView(InputFileState inputFileState)
    {
        return inputFileState.OpenedTreeViewModelHistory[
            inputFileState.IndexInHistory];
    }
    
    private void ChangeContentRootToOpenedTreeView(
        InputFileState inputFileState)
    {
        var openedTreeView = GetOpenedTreeView(InputFileStateWrap.Value);
        
        if (openedTreeView.Item is not null)
            SetInputFileContentTreeViewRoot.Invoke(openedTreeView.Item);
    }

    protected override void Dispose(bool disposing)
    {
        InputFileStateWrap.StateChanged -= InputFileStateWrapOnStateChanged;
        
        base.Dispose(disposing);
    }
}