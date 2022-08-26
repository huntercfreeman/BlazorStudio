using System.Net.NetworkInformation;
using System.Text;
using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Html;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.RoslynHelpers;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.DragCase;
using BlazorStudio.ClassLib.Store.KeyDownEventCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.ClassLib.Virtualize;
using BlazorStudio.RazorLib.ContextCase;
using BlazorStudio.RazorLib.VirtualizeComponentExperiments;
using BlazorStudio.RazorLib.VirtualizeComponents;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
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
    /// <summary>
    /// -1 means only focusable through JavaScript
    ///<br/>--<br/>
    /// 0 or >0 means focusable through hitting 'Tab' key
    ///<br/>--<br/>
    /// This Parameter is here because after setting focus the the PlainTextEditor one
    /// no longer can use the 'Tab' key to change focus. One would have to use a separate
    /// custom made keymap to move focus out of the editor or click with the mouse out of the Editor. 
    ///<br/>--<br/>
    /// I recommend TabIndex = 0 however the default is TabIndex = -1 to ensure nobody unknowingly gets
    /// 'focus trapped' in the editor.
    /// </summary>
    [Parameter]
    public int TabIndex { get; set; } = -1;

    private bool _isFocused;
    private ElementReference? _plainTextEditor;
    private ContextBoundary? _contextBoundary;
    private int _hadOnKeyDownEventCounter;
    private Virtualize<(int Index, IPlainTextEditorRow PlainTextEditorRow)>? _virtualize;
    private VirtualizeCoordinateSystemExperimental<(int Index, IPlainTextEditorRow PlainTextEditorRow)>? _virtualizeCoordinateSystemExperimental;
    private int _previousFontSize;
    private string _widthAndHeightTestId = "bstudio_pte-get-width-and-height-test";
    
    private bool _isMouseSelectingText;
    private SemaphoreSlim _mouseTextSelectionSemaphoreSlim = new(1, 1);
    
    private SequenceKey? _previousSequenceKeyShouldRender;
    private PlainTextEditorKey? _previousPlainTextEditorKey;

    private PlainTextEditorCursorDisplay? _plainTextEditorCursorDisplay;

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
    private int _focusCursorAsyncClickCount;

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
            if (_plainTextEditorCursorDisplay is not null)
            {
                await JsRuntime.InvokeVoidAsync(
                    "plainTextEditor.initializeIntersectionObserverForCursorOffscreen",
                    PlainTextEditorDisplayId,
                    _plainTextEditorCursorDisplay.CursorElementId);
            }

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
            if (_virtualize is not null)
            {
                _previousFontSize = 0;
                PlainTextEditorSelectorOnSelectedValueChanged(null, null);
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
            // rerender and measure the new character width / row height values
            {
                _isInitialized = false;
                _previousSequenceKeyShouldRender = null;
            
                await InvokeAsync(StateHasChanged);
            
                _widthAndHeightTestResult = await JsRuntime.InvokeAsync<WidthAndHeightTestResult>(
                    "plainTextEditor.widthAndHeightTest",
                    _widthAndHeightTestId);    

                _isInitialized = true;
            }
            
            _previousFontSize = currentPlainTextEditor.RichTextEditorOptions.FontSizeInPixels;
            
            if (_virtualize is not null)
                await _virtualize.RefreshDataAsync();
            
            _previousSequenceKeyShouldRender = null;

            Dispatcher.Dispatch(new PlainTextEditorSetOptionsAction(currentPlainTextEditor.PlainTextEditorKey,
                currentPlainTextEditor.RichTextEditorOptions with
                {
                    WidthOfACharacterInPixels = _widthAndHeightTestResult.WidthOfACharacter,
                    HeightOfARowInPixels = _widthAndHeightTestResult.HeightOfARow,
                    WidthOfEditorInPixels = _widthAndHeightTestResult.WidthOfEditor,
                    HeightOfEditorInPixels = _widthAndHeightTestResult.HeightOfEditor
                }));
        }
        
        if (_virtualize is not null)
            await _virtualize.RefreshDataAsync();
            
        _previousSequenceKeyShouldRender = null;
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        _isMouseSelectingText = false;

        if (e.AltKey)
        {
            return;
        }
        
        var keyDownEventRecord = new KeyDownEventRecord(
            e.Key,
            e.Code,
            e.CtrlKey,
            e.ShiftKey,
            e.AltKey
        );

        if (e.CtrlKey)
        {
            if (_contextBoundary is not null &&
                _contextBoundary.GetContextState.Keymap.Map.TryGetValue(keyDownEventRecord, out var command))
            {
                Dispatcher.Dispatch(new KeymapEventAction(
                    keyDownEventRecord,
                    PlainTextEditorKey,
                    CancellationToken.None));

                return;
            }
        }

        Dispatcher.Dispatch(
            new KeyDownEventAction(PlainTextEditorKey,
                keyDownEventRecord,
                CancellationToken.None
            )
        );

        if (_plainTextEditorCursorDisplay is not null)
        {
            await JsRuntime.InvokeVoidAsync(
                "plainTextEditor.scrollCursorIntoViewIfOutOfViewport",
                _plainTextEditorCursorDisplay.CursorElementId);
        }
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

    private async Task FocusCursorAsync()
    {
        _focusCursorAsyncClickCount++;
        
        if (_plainTextEditorCursorDisplay is not null)
        {
            await _plainTextEditorCursorDisplay.FocusAsync();
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        PlainTextEditorSelector.SelectedValueChanged -= PlainTextEditorSelectorOnSelectedValueChanged;

        if (_plainTextEditorCursorDisplay is not null)
        {
            _ = Task.Run(async () =>
            {
                await JsRuntime.InvokeVoidAsync(
                    "plainTextEditor.disposeIntersectionObserverForCursorOffscreen",
                    _plainTextEditorCursorDisplay.CursorElementId);    
            });
        }

        if (AutomateDispose)
            Dispatcher.Dispatch(new DeconstructPlainTextEditorRecordAction(PlainTextEditorKey));

        base.Dispose(disposing);
    }

    public class WidthAndHeightTestResult
    {
        public double WidthOfACharacter { get; set; }
        public double HeightOfARow { get; set; }
        public double WidthOfEditor { get; set; }
        public double HeightOfEditor { get; set; }
    }

    private ICollection<(int Index, IPlainTextEditorRow PlainTextEditorRow)> GetItems(IPlainTextEditor currentPlainTextEditor)
    {
        return currentPlainTextEditor.Rows
            .Select((row, index) => (index, row))
            .ToArray();
    }

    private VirtualizeItemDimensions GetVirtualizeItemDimensions(IPlainTextEditor currentPlainTextEditor)
    {
        return new VirtualizeItemDimensions(
            currentPlainTextEditor.RichTextEditorOptions.WidthOfACharacterInPixels,
            currentPlainTextEditor.RichTextEditorOptions.HeightOfARowInPixels,
            currentPlainTextEditor.RichTextEditorOptions.WidthOfEditorInPixels,
            currentPlainTextEditor.RichTextEditorOptions.HeightOfEditorInPixels);
    }

    private async ValueTask<ItemsProviderResult<(int Index, IPlainTextEditorRow PlainTextEditorRow)>> RowItemsProvider(
        ItemsProviderRequest request)
    {
        var currentPlainTextEditor = PlainTextEditorSelector.Value;

        (int Index, IPlainTextEditorRow PlainTextEditorRow)[] rowTuples =
            Array.Empty<(int Index, IPlainTextEditorRow PlainTextEditorRow)>();
        
        var numberOfRows = Math.Min(request.Count, currentPlainTextEditor.Rows.Count - request.StartIndex);

        if (numberOfRows > 0)
        {
            if (request.CancellationToken.IsCancellationRequested)
                return default;
            
            rowTuples = currentPlainTextEditor.Rows
                .Select((row, index) => (index, row))
                .Skip(request.StartIndex)
                .Take(numberOfRows)
                .ToArray();

            return new ItemsProviderResult<(int Index, IPlainTextEditorRow PlainTextEditorRow)>(rowTuples,
                currentPlainTextEditor.FileHandle.VirtualRowCount);
        }

        return new ItemsProviderResult<(int Index, IPlainTextEditorRow PlainTextEditorRow)>(rowTuples,
            0);
    }
}
