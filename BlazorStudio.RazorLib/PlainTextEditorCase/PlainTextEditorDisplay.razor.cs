using System.Text;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Html;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.RoslynHelpers;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.DragCase;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.ClassLib.Virtualize;
using BlazorStudio.RazorLib.VirtualizeComponentExperiments;
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
    private IState<SolutionState> SolutionStateWrap { get; set; } = null!;
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
    private ElementReference? _plainTextEditor;
    private int _hadOnKeyDownEventCounter;
    private VirtualizeCoordinateSystemExperimental<(int Index, IPlainTextEditorRow PlainTextEditorRow)>? _virtualizeCoordinateSystemExperimental;
    private int _previousFontSize;
    private string _widthAndHeightTestId = "bstudio_pte-get-width-and-height-test";
    
    private bool _isMouseSelectingText;
    private SemaphoreSlim _mouseTextSelectionSemaphoreSlim = new(1, 1);
    
    private SequenceKey? _previousSequenceKeyShouldRender;
    private PlainTextEditorKey? _previousPlainTextEditorKey;

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
    private WidthAndHeightTestResult _widthAndHeightTestResult;

    private string IsFocusedCssClass => _isFocused
        ? "pte_focused"
        : "";

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
            PlainTextEditorSelectorOnSelectedValueChanged(null, null);
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

        bool shouldRender = plainTextEditor.SequenceKey != _previousSequenceKeyShouldRender;

        _previousSequenceKeyShouldRender = plainTextEditor.SequenceKey;

        if (plainTextEditor.PlainTextEditorKey != _previousPlainTextEditorKey)
        {
            // Parameter changed and the VirtualizeCoordinateSystem must reset
            if (_virtualizeCoordinateSystemExperimental is not null)
            {
                _previousFontSize = 0;
                _isInitialized = false;
                PlainTextEditorSelectorOnSelectedValueChanged(null, null);
                //_virtualizeCoordinateSystem.ResetState();
            }
        }

        if (_isMouseSelectingText)
        {
            shouldRender = true;
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
            _widthAndHeightTestResult = await JsRuntime.InvokeAsync<WidthAndHeightTestResult>(
                "plainTextEditor.widthAndHeightTest",
                    _widthAndHeightTestId);

            _previousSequenceKeyShouldRender = null;

            _isInitialized = true;

            _previousFontSize = currentPlainTextEditor.RichTextEditorOptions.FontSizeInPixels;

            Dispatcher.Dispatch(new PlainTextEditorSetOptionsAction(currentPlainTextEditor.PlainTextEditorKey,
                currentPlainTextEditor.RichTextEditorOptions with
                {
                    WidthOfACharacterInPixels = _widthAndHeightTestResult.WidthOfACharacter,
                    HeightOfARowInPixels = _widthAndHeightTestResult.HeightOfARow
                }));
        }
    }

    private async Task OnAfterFirstRenderCallbackFunc()
    {
        //await JsRuntime.InvokeVoidAsync("plainTextEditor.subscribeScrollIntoView",
        //    ActiveRowPositionMarkerId,
        //    PlainTextEditorKey.Guid);
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        _isMouseSelectingText = false;
        
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
        _previousSequenceKeyShouldRender = null;
        _isFocused = false;
    }

    private void FocusPlainTextEditorOnClick()
    {
        // Backup way to ensure DragEvents stop
        _isMouseSelectingText = false;
        
        _previousSequenceKeyShouldRender = null;

        FocusPlainTextEditor();
    }
    
    private void FocusPlainTextEditor()
    {
        if (_plainTextEditor is not null)
        {
            _plainTextEditor.Value.FocusAsync();
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

    private void StartSelectingText()
    {
        _isMouseSelectingText = true;
    }
    
    private void StopSelectingText()
    {
        _isMouseSelectingText = false;
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

    private ICollection<(int Index, IPlainTextEditorRow PlainTextEditorRow)> GetItems(IPlainTextEditor currentPlainTextEditor)
    {
        return currentPlainTextEditor.Rows
            .Select((row, index) => (index, row))
            .ToArray();
    }
}
