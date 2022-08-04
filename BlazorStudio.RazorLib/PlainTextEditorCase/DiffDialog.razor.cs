﻿using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class DiffDialog : FluxorComponent
{
    [Inject]
    private IStateSelection<PlainTextEditorStates, IPlainTextEditor?> PlainTextEditorSelector { get; set; } = null!;

    [Parameter, EditorRequired]
    public PlainTextEditorKey PlainTextEditorKey { get; set; } = null!;

    private string _plainText = string.Empty;

    protected override void OnInitialized()
    {
        PlainTextEditorSelector.Select(x =>
        {
            x.Map.TryGetValue(PlainTextEditorKey, out var value);
            return value;
        });

        base.OnInitialized();
    }

    private void GetPlainText(IPlainTextEditor currentPlainTextEditor)
    {
        _plainText = currentPlainTextEditor.GetPlainText();
    }

    private void SaveChanges()
    {

    }
}