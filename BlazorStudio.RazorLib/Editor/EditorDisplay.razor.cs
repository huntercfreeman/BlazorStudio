﻿using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.RazorLib.PlainTextEditorCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Editor;

public partial class EditorDisplay : FluxorComponent
{
    [Inject]
    private IState<EditorState> EditorStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public ClassLib.UserInterface.Dimensions Dimensions { get; set; } = null!;

    private PlainTextEditorKey _plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

    protected override void OnInitialized()
    {
        EditorStateWrap.StateChanged += EditorStateWrap_StateChanged;

        base.OnInitialized();
    }

    private async void EditorStateWrap_StateChanged(object? sender, EventArgs e)
    {
        Dispatcher.Dispatch(new DeconstructPlainTextEditorRecordAction(_plainTextEditorKey));

        _plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

        Dispatcher.Dispatch(
            new ConstructMemoryMappedFilePlainTextEditorRecordAction(_plainTextEditorSpawn.PlainTextEditorKey,
                EditorStateWrap.Value.OpenedAbsoluteFilePath)
        );
    }
}