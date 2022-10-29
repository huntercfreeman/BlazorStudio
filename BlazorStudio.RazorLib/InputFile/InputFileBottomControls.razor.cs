using System.Collections.Immutable;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.TreeView;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileBottomControls : FluxorComponent
{
    [Inject]
    private IState<InputFileState> InputFileStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [CascadingParameter]
    public DialogRecord? DialogRecord { get; set; }

    private ElementReference? _searchElementReference;
    private string _searchQuery = string.Empty;

    private TreeViewModel<IAbsoluteFilePath> SelectionMutablyReferenced => 
        InputFileStateWrap.Value.OpenedTreeViewModelHistory[
            InputFileStateWrap.Value.IndexInHistory];
    
    private void HandleBackButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.MoveBackwardsInHistoryAction());
    }
    
    private void HandleForwardButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.MoveForwardsInHistoryAction());
    }

    private void HandleUpwardButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.OpenParentDirectoryAction());
    }

    private void HandleRefreshButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.RefreshCurrentSelectionAction());
    }

    private void FocusSearchElementReferenceOnClick()
    {
        _searchElementReference?.FocusAsync();
    }

    private async Task FireOnAfterSubmit()
    {
        var inputFileState = InputFileStateWrap.Value;

        var valid = await inputFileState.SelectionIsValidFunc.Invoke(
            inputFileState.SelectedTreeViewModel?.Item);
        
        if (valid)
        {
            if (DialogRecord is not null)
                Dispatcher.Dispatch(new DisposeDialogRecordAction(DialogRecord));
            
            await InputFileStateWrap.Value.OnAfterSubmitFunc
                .Invoke(inputFileState.SelectedTreeViewModel?.Item);
        }
    }
    
    private bool OnAfterSubmitIsValid()
    {
        var inputFileState = InputFileStateWrap.Value;

        return inputFileState.SelectionIsValidFunc.Invoke(
            inputFileState.SelectedTreeViewModel?.Item)
            .Result;
    }
}