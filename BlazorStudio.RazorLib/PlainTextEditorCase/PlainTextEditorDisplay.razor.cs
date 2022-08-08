using System.Text;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Html;
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
    [Parameter, EditorRequired]
    public bool AutomateDispose { get; set; } = true;
    [Parameter]
    public bool AllowOpenDiff { get; set; } = true;
    [Parameter]
    public bool AllowDispatchEvent { get; set; } = true;

    private bool _isFocused;
    private ElementReference _plainTextEditor;
    private int _hadOnKeyDownEventCounter;
    private VirtualizeCoordinateSystem<(int Index, IPlainTextEditorRow PlainTextEditorRow)>? _virtualizeCoordinateSystem;
    private int _previousFontSize;
    private string _widthAndHeightTestId = "bstudio_pte-get-width-and-height-test";

    private SequenceKey? _previousSequenceKeyShouldRender;
    private PlainTextEditorKey? _previousPlainTextEditorKey;
    private ElementReference? _activePositionMarker;
    private double _heightOfEachRowInPixels = 30;
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
            },
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.CharacterHeight,
                Value = -1.5
            },
        }
    };

    private string PlainTextEditorDisplayId => $"pte_plain-text-editor-display_{PlainTextEditorKey.Guid}";
    private string ActiveRowPositionMarkerId => $"pte_active-row-position-marker_{PlainTextEditorKey.Guid}";
    private string ActiveRowId => $"pte_active-row_{PlainTextEditorKey.Guid}";
    private bool _isInitialized;

    private string IsFocusedCssClass => _isFocused
        ? "pte_focused"
        : "";

    private string GetActivePositionMarkerDimensions(IPlainTextEditor currentPlainTextEditor)
    {
        var styleCssBuilder = new StringBuilder();

        styleCssBuilder.Append($"position: absolute; ");

        styleCssBuilder.Append($"width: {_widthOfEachCharacterInPixels}px; ");
        styleCssBuilder.Append($"height: {_heightOfEachRowInPixels}px; ");
        styleCssBuilder.Append($"left: calc(3ch + {_widthOfEachCharacterInPixels * currentPlainTextEditor.CurrentCharacterColumnIndex}px); ");
        styleCssBuilder.Append($"top: {_heightOfEachRowInPixels * (currentPlainTextEditor.CurrentRowIndex)}px; ");

        return styleCssBuilder.ToString();
    }

    private string GetActivePositionMarkerLeft(int characterIndex) => $"left: {_widthOfEachCharacterInPixels * characterIndex}px;";
    private string GetActivePositionMarkerTop(int rowIndex) => $"top: {_heightOfEachRowInPixels * rowIndex}px;";

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

            //await JsRuntime.InvokeVoidAsync("plainTextEditor.scrollIntoViewIfOutOfViewport",
            //    ActiveRowPositionMarkerId);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// @onkeydown by default takes an EventCallback which causes
    /// many redundant StateHasChanged calls.
    /// 
    /// Fluxor IStateSelection correctly does not render in certain conditions but
    /// the EventCallback implicitly calling StateHasChanged results in ShouldRender being necessary
    /// </summary>
    /// <returns></returns>
    protected override bool ShouldRender()
    {
        var plainTextEditor = PlainTextEditorSelector.Value;

        if (plainTextEditor is null)
            return true;

        var shouldRender = false;

        if (plainTextEditor.SequenceKey != _previousSequenceKeyShouldRender)
            shouldRender = true;

        _previousSequenceKeyShouldRender = plainTextEditor.SequenceKey;

        if (plainTextEditor.PlainTextEditorKey != _previousPlainTextEditorKey)
        {
            // Parameter changed and the VirtualizeCoordinateSystem must reset
            if (_virtualizeCoordinateSystem is not null)
            {
                _virtualizeCoordinateSystem.ResetState();
            }
        }

        _previousPlainTextEditorKey = plainTextEditor.PlainTextEditorKey;

        return shouldRender;
    }

    private async void PlainTextEditorSelectorOnSelectedValueChanged(object? sender, IPlainTextEditor? e)
    {
        var currentPlainTextEditor = PlainTextEditorSelector.Value;

        if (currentPlainTextEditor is null)
            return;

        if (_previousFontSize != currentPlainTextEditor.RichTextEditorOptions.FontSizeInPixels)
        {
            var widthAndHeightTestResult = 
                JsRuntime.InvokeAsync<WidthAndHeightTestResult>("plainTextEditor.widthAndHeightTest",
                    _widthAndHeightTestId);

            var z = 2;
        }

        _previousFontSize = currentPlainTextEditor.RichTextEditorOptions.FontSizeInPixels;
    }

    private async Task OnAfterFirstRenderCallbackFunc()
    {
        //await JsRuntime.InvokeVoidAsync("plainTextEditor.subscribeScrollIntoView",
        //    ActiveRowPositionMarkerId,
        //    PlainTextEditorKey.Guid);
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
                ),
                CancellationToken.None
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
        //_previousSequenceKeyShouldRender = null;
        //_isFocused = false;
    }

    private void FocusPlainTextEditorOnClick()
    {
        _previousSequenceKeyShouldRender = null;

        if (_activePositionMarker is not null)
        {
            _activePositionMarker.Value.FocusAsync();
        }

    }

    private string GetStyleCss()
    {
        return $"font-size: {PlainTextEditorSelector.Value?.RichTextEditorOptions.FontSizeInPixels ?? 0}px;";
    }

    private void OnRequestCallbackAction(VirtualizeCoordinateSystemMessage virtualizeCoordinateSystemMessage)
    {
        if (AllowDispatchEvent)
        {
            Dispatcher.Dispatch(new PlainTextEditorPixelReadRequestAction(PlainTextEditorKey,
                virtualizeCoordinateSystemMessage));
        }
    }

    private MarkupString NullSafeToMarkupString(string name, object? obj)
    {
        return (MarkupString) (name + "&nbsp;->&nbsp;" + obj?.ToString() ?? "null");
    }

    private void IsReadonlyCheckboxOnChange(ChangeEventArgs e)
    {
        var plainTextEditorKey = PlainTextEditorSelector.Value.PlainTextEditorKey;

        Dispatcher.Dispatch(new SetIsReadonlyAction(plainTextEditorKey, (bool) (e.Value ?? true)));
    }
    
    private async Task SaveChangesOnClick()
    {
        var currentPlainTextEditor = PlainTextEditorSelector.Value;

        await currentPlainTextEditor.FileHandle.SaveAsync("", CancellationToken.None);

        Dispatcher.Dispatch(new PlainTextEditorPixelReadRequestAction(PlainTextEditorKey,
            currentPlainTextEditor.VirtualizeCoordinateSystemMessage));
    }

    protected override void Dispose(bool disposing)
    {
        PlainTextEditorSelector.SelectedValueChanged -= PlainTextEditorSelectorOnSelectedValueChanged;

        //_ = Task.Run(() => JsRuntime.InvokeVoidAsync("plainTextEditor.disposeScrollIntoView",
        //    ActiveRowPositionMarkerId));

        if (AutomateDispose)
            Dispatcher.Dispatch(new DeconstructPlainTextEditorRecordAction(PlainTextEditorKey));

        base.Dispose(disposing);
    }

    public class WidthAndHeightTestResult
    {
        public double HeightOfARow { get; set; }
        public double WidthOfACharacter { get; set; }
    }
}
