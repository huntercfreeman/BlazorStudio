﻿using System.Collections.Immutable;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
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
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    
    [CascadingParameter]
    public DialogRecord? DialogRecord { get; set; }

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
        var inputFileState = InputFileStateWrap.Value;
        
        var patternName = (string)(changeEventArgs.Value ?? string.Empty);

        var pattern = inputFileState.InputFilePatterns
            .FirstOrDefault(x => x.PatternName == patternName);

        if (pattern is not null)
        {
            Dispatcher.Dispatch(
                new InputFileState.SetSelectedInputFilePatternAction(
                    pattern));            
        }
    }
}