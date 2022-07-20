using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorStudio.Shared.FileSystem.Classes;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using PlainTextEditor.ClassLib.Keyboard;
using PlainTextEditor.ClassLib.Sequence;
using PlainTextEditor.ClassLib.Store.KeyDownEventCase;
using PlainTextEditor.ClassLib.Store.PlainTextEditorCase;

namespace PlainTextEditor.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorDisplay : FluxorComponent, IDisposable
{
    [Inject]
    private IStateSelection<PlainTextEditorStates, IPlainTextEditor?> PlainTextEditorSelector { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter, EditorRequired]
    public PlainTextEditorKey PlainTextEditorKey { get; set; } = null!;

    private bool _isFocused;
    private ElementReference _plainTextEditor;
    private Virtualize<(int Index, IPlainTextEditorRow PlainTextEditorRow)> _rowVirtualizeComponent = null!;
    private int _hadOnKeyDownEventCounter;

    private string PlainTextEditorDisplayId => $"pte_plain-text-editor-display_{PlainTextEditorKey.Guid}";
    private string ActiveRowPositionMarkerId => $"pte_active-row-position-marker_{PlainTextEditorKey.Guid}";
    private string ActiveRowId => $"pte_active-row_{PlainTextEditorKey.Guid}";

    private string IsFocusedCssClass => _isFocused
        ? "pte_focused"
        : "";

    /// <summary>
    /// I need to position this PERFECTLY relative to a changeable font-size
    /// </summary>
    private string InputFocusTrapTopStyleCss => $"top: calc({PlainTextEditorSelector.Value!.CurrentRowIndex * 30}px);";

    private SequenceKey? _previousSequenceKey;

    protected override void OnInitialized()
    {
        PlainTextEditorSelector.Select(x => 
        {
            x.Map.TryGetValue(PlainTextEditorKey, out var value);
            return value;
        });

        PlainTextEditorSelector.SelectedValueChanged += PlainTextEditorSelectorOnSelectedValueChanged;

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("plainTextEditor.subscribeScrollIntoView",
                ActiveRowPositionMarkerId,
                PlainTextEditorKey.Guid);
        }

        if (_hadOnKeyDownEventCounter > 0)
        {
            _hadOnKeyDownEventCounter = 0;

            await JsRuntime.InvokeVoidAsync("plainTextEditor.scrollIntoViewIfOutOfViewport",
                ActiveRowPositionMarkerId);
            
            Console.WriteLine("scrollIntoViewIfOutOfViewport");
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// @onkeydown by default takes an EventCallback which causes
    /// many redundant StateHasChanged calls.
    /// 
    /// Fluxor IStateSelection correctly does not render in certain conditions but
    /// the EventCallback implicitely calling StateHasChanged results in ShouldRender being necessary
    /// </summary>
    /// <returns></returns>
    protected override bool ShouldRender()
    {
        if (PlainTextEditorSelector.Value is null)
            return true;

        var shouldRender = false;

        if(PlainTextEditorSelector.Value.SequenceKey != _previousSequenceKey)
            shouldRender = true;

        _previousSequenceKey = PlainTextEditorSelector.Value.SequenceKey;

        return shouldRender;
    }

    private async void PlainTextEditorSelectorOnSelectedValueChanged(object? sender, IPlainTextEditor? e)
    {
        await _rowVirtualizeComponent.RefreshDataAsync();

        await InvokeAsync(StateHasChanged);
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        // I need to fix scroll into view after keypress it is not working
        // F1 is a temporary hack due to as of this comment
        // the scrolling occurs EVERY key press.
        //
        // It should only scroll into view if key pressed and active
        // row is out of viewport NOT every key press.
        if (e.Key == "F1")
        {
            _hadOnKeyDownEventCounter++;
        }

        Dispatcher.Dispatch(
            new KeyDownEventAction(PlainTextEditorKey,
                new ClassLib.Keyboard.KeyDownEventRecord(
                    e.Key,
                    e.Code,
                    e.CtrlKey,
                    e.ShiftKey,
                    e.AltKey
                )
            )
        );
    }
    
    private void OnFocusIn()
    {
        _previousSequenceKey = null;
        _isFocused = true;
    }

    private void OnFocusOut()
    {
        _previousSequenceKey = null;
        _isFocused = false;
    }

    private void FocusPlainTextEditorOnClick()
    {
        _previousSequenceKey = null;
        _plainTextEditor.FocusAsync();
    }

    private string GetStyleCss()
    {
        return $"font-size: {PlainTextEditorSelector.Value?.RichTextEditorOptions.FontSizeInPixels ?? 0}px;";
    }

    private async ValueTask<ItemsProviderResult<(int Index, IPlainTextEditorRow PlainTextEditorRow)>> RowItemsProvider(
        ItemsProviderRequest request)
    {
        var currentPlainTextEditor = PlainTextEditorSelector.Value;

        (int Index, IPlainTextEditorRow PlainTextEditorRow)[] rowTuples =
            Array.Empty<(int Index, IPlainTextEditorRow PlainTextEditorRow)>();

        if (currentPlainTextEditor is null)
            return ValueTask.FromResult(new ItemsProviderResult<(int Index, IPlainTextEditorRow PlainTextEditorRow)>(rowTuples,
                0));

        var numberOfRows = Math.Min(request.Count, currentPlainTextEditor.List.Count - request.StartIndex);

        if (numberOfRows > 0)
        {
            var rowIndexTextTuples = await currentPlainTextEditor.FileCoordinateGrid
                .Request(new FileCoordinateGridRequest(request.StartIndex, numberOfRows, request.CancellationToken));
        }

        return ValueTask.FromResult(new ItemsProviderResult<(int Index, IPlainTextEditorRow PlainTextEditorRow)>(rowTuples,
            currentPlainTextEditor.FileCoordinateGrid.RowCount));
    }

    protected override void Dispose(bool disposing)
    {
        PlainTextEditorSelector.SelectedValueChanged -= PlainTextEditorSelectorOnSelectedValueChanged;

        _ = Task.Run(() => JsRuntime.InvokeVoidAsync("plainTextEditor.disposeScrollIntoView",
            ActiveRowPositionMarkerId));

        base.Dispose(disposing);
    }
}
