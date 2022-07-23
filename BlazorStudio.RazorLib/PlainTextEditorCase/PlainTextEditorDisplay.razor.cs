using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.ClassLib.Virtualize;
using BlazorStudio.RazorLib.VirtualizeComponents;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

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
    private int _hadOnKeyDownEventCounter;
    private VirtualizeCoordinateSystem<(int Index, IPlainTextEditorRow PlainTextEditorRow)> _virtualizeCoordinateSystem = null!;
    private int _forceRerender;

    private SequenceKey? _previousSequenceKeyShouldRender;

    private double _heightOfEachRowInPixels = 27;
    private double _widthOfEachCharacterInPixels = 9.91;

    private Dimensions _dimensionsOfCoordinateSystemViewport = new()
    {
        DimensionsPositionKind = DimensionsPositionKind.Relative,
        WidthCalc = new List<DimensionUnit>
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 100
            }
        },
        HeightCalc = new List<DimensionUnit>
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Percentage,
                Value = 100
            }
        }
    };

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
        if (_hadOnKeyDownEventCounter > 0)
        {
            _hadOnKeyDownEventCounter = 0;

            await JsRuntime.InvokeVoidAsync("plainTextEditor.scrollIntoViewIfOutOfViewport",
                ActiveRowPositionMarkerId);
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

        if (PlainTextEditorSelector.Value.SequenceKey != _previousSequenceKeyShouldRender)
            shouldRender = true;

        _previousSequenceKeyShouldRender = PlainTextEditorSelector.Value.SequenceKey;

        return shouldRender;
    }

    private void PlainTextEditorSelectorOnSelectedValueChanged(object? sender, IPlainTextEditor? e)
    {
        var plainTextEditor = PlainTextEditorSelector.Value;

        if (plainTextEditor is null)
            return;

        _virtualizeCoordinateSystem.SetData(plainTextEditor.VirtualizeCoordinateSystemMessage);
    }

    private async Task OnAfterFirstRenderCallbackFunc()
    {
        await JsRuntime.InvokeVoidAsync("plainTextEditor.subscribeScrollIntoView",
            ActiveRowPositionMarkerId,
            PlainTextEditorKey.Guid);
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
                new KeyDownEventRecord(
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
        _previousSequenceKeyShouldRender = null;
        _isFocused = true;
    }

    private void OnFocusOut()
    {
        _previousSequenceKeyShouldRender = null;
        _isFocused = false;
    }

    private void FocusPlainTextEditorOnClick()
    {
        _previousSequenceKeyShouldRender = null;
        _plainTextEditor.FocusAsync();
    }

    private string GetStyleCss()
    {
        return $"font-size: {PlainTextEditorSelector.Value?.RichTextEditorOptions.FontSizeInPixels ?? 0}px;";
    }

    private void OnRequestCallbackAction(VirtualizeCoordinateSystemMessage virtualizeCoordinateSystemMessage)
    {
        Dispatcher.Dispatch(new MemoryMappedFileReadRequestAction(PlainTextEditorKey,
            virtualizeCoordinateSystemMessage));
    }

    private string NullSafeToString(string name, object? obj)
    {
        return $"{name}&nbsp;->&nbsp;{obj?.ToString() ?? "null"}";
    }

    protected override void Dispose(bool disposing)
    {
        PlainTextEditorSelector.SelectedValueChanged -= PlainTextEditorSelectorOnSelectedValueChanged;

        _ = Task.Run(() => JsRuntime.InvokeVoidAsync("plainTextEditor.disposeScrollIntoView",
            ActiveRowPositionMarkerId));

        base.Dispose(disposing);
    }
}
