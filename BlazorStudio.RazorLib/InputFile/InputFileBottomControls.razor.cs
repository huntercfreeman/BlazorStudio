using System.Collections.Immutable;
using BlazorALaCarte.DialogNotification;
using BlazorALaCarte.DialogNotification.Dialog;
using BlazorALaCarte.DialogNotification.Store.DialogCase;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileBottomControls : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    
    [CascadingParameter]
    public DialogRecord? DialogRecord { get; set; }
    [CascadingParameter]
    public InputFileState InputFileState { get; set; } = null!;

    private ElementReference? _searchElementReference;
    private string _searchQuery = string.Empty;
    
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
        Dispatcher.Dispatch(new InputFileState.OpenParentDirectoryAction(
            CommonComponentRenderers));
    }

    private void HandleRefreshButtonOnClick()
    {
        Dispatcher.Dispatch(new InputFileState.RefreshCurrentSelectionAction());
    }

    private void FocusSearchElementReferenceOnClick()
    {
        _searchElementReference?.FocusAsync();
    }
    
    private void SelectInputFilePatternOnChange(ChangeEventArgs changeEventArgs)
    {
        var patternName = (string)(changeEventArgs.Value ?? string.Empty);

        var pattern = InputFileState.InputFilePatterns
            .FirstOrDefault(x => x.PatternName == patternName);

        if (pattern is not null)
        {
            Dispatcher.Dispatch(
                new InputFileState.SetSelectedInputFilePatternAction(
                    pattern));            
        }
    }

    private string GetSelectedTreeViewModelAbsoluteFilePathString(InputFileState inputFileState)
    {
        var selectedAbsoluteFilePath = inputFileState.SelectedTreeViewModel?.Item;

        if (selectedAbsoluteFilePath is null)
            return "Selection is null";
        
        return selectedAbsoluteFilePath.GetAbsoluteFilePathString();
    }
    
    private async Task FireOnAfterSubmit()
    {
        var valid = await InputFileState.SelectionIsValidFunc.Invoke(
            InputFileState.SelectedTreeViewModel?.Item);
        
        if (valid)
        {
            if (DialogRecord is not null)
            {
                Dispatcher.Dispatch(
                    new DialogRecordsCollection.DisposeAction(
                        DialogRecord.DialogKey));
            }
            
            await InputFileState.OnAfterSubmitFunc
                .Invoke(InputFileState.SelectedTreeViewModel?.Item);
        }
    }
    
    private bool OnAfterSubmitIsDisabled()
    {
        return !InputFileState.SelectionIsValidFunc.Invoke(
                InputFileState.SelectedTreeViewModel?.Item)
            .Result;
    }
    
    private Task CancelOnClick()
    {
        if (DialogRecord is not null)
        {
            Dispatcher.Dispatch(
                new DialogRecordsCollection.DisposeAction(
                    DialogRecord.DialogKey));
        }

        return Task.CompletedTask;
    }
}