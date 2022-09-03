using System.Collections.Immutable;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.RoslynHelpers;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.TextEditorCase;
using BlazorStudio.ClassLib.TextEditor;
using BlazorStudio.ClassLib.TextEditor.Cursor;
using BlazorStudio.ClassLib.TextEditor.Enums;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.RazorLib.CustomEvents;
using BlazorStudio.RazorLib.CustomJavaScriptDtos;
using BlazorStudio.RazorLib.ShouldRender;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.JSInterop;

namespace BlazorStudio.RazorLib.TextEditorCase;

public partial class TextEditorDisplay : FluxorComponent
{
    [Inject]
    private IStateSelection<TextEditorStates, TextEditorBase> TextEditorStatesSelection { get; set; } = null!;
    [Inject] 
    private IState<TextEditorOptionsState> TextEditorOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorKey TextEditorKey { get; set; } = null!;

    private Guid _textEditorGuid = Guid.NewGuid();
    private TextPartition? _textPartition;
    private SequenceKey _previousTextPartitionSequenceKey = SequenceKey.Empty();
    private TextEditorKey _previousTextEditorKey = TextEditorKey.Empty();
    private TextCursor _cursor = new();
    private TextEditorCursorDisplay? _textEditorCursorDisplay;
    private Virtualize<TextCharacterSpan>? _virtualize;
    private RelativeCoordinates? _mostRecentRelativeCoordinates;
    private int _renderCount;
    private bool _shouldMeasureFontSize = true;
    private TextEditorFontSize? _textEditorFontSize;
    private int _fontSizeMeasurementMultiplier = 6; 
    private string _fontSizeMeasurementTestData = "abcdefghijklmnopqrstuvwxyz0123456789";

    private string GetTextEditorElementId => $"bstudio_{_textEditorGuid}";
    private string GetMeasureFontSizeElementId => $"{GetTextEditorElementId}-measure-font-size";

    private string GetFontSize => $"{TextEditorOptionsStateWrap.Value.FontSize.Value}" +
                                  $"{TextEditorOptionsStateWrap.Value.FontSize.DimensionUnitKind.ToCssString()}";

    protected override void OnInitialized()
    {
        TextEditorStatesSelection
            .Select(x =>
                x.TextEditors.SingleOrDefault(x => x.TextEditorKey == TextEditorKey));

        TextEditorOptionsStateWrap.StateChanged += TextEditorOptionsStateWrapOnStateChanged;
        
        base.OnInitialized();
    }

    private void TextEditorOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        _shouldMeasureFontSize = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_previousTextEditorKey != TextEditorStatesSelection.Value.TextEditorKey &&
            _virtualize is not null)
        {
            await _virtualize.RefreshDataAsync();

            _previousTextEditorKey = TextEditorStatesSelection.Value.TextEditorKey;

            await InvokeAsync(StateHasChanged);
        }

        if (_shouldMeasureFontSize)
        {
            _textEditorFontSize = await JsRuntime.InvokeAsync<TextEditorFontSize>(
                "blazorStudio.measureFontSizeByElementId", 
                GetMeasureFontSizeElementId,
                _fontSizeMeasurementTestData.Length * _fontSizeMeasurementMultiplier);

            _shouldMeasureFontSize = false;
            
            _previousTextPartitionSequenceKey = SequenceKey.Empty();

            await InvokeAsync(StateHasChanged);
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private bool ShouldRenderFunc(ShouldRenderBoundary.IsFirstShouldRenderValue firstShouldRender)
    {
        var shouldRender = TextEditorStatesSelection.Value is not null &&
                           (_textPartition is null || (_previousTextPartitionSequenceKey != _textPartition.SequenceKey));

        _previousTextPartitionSequenceKey = _textPartition?.SequenceKey ?? SequenceKey.Empty();

        return shouldRender;
    }

    /// <summary>
    /// For multi cursor I imagine one would foreach() loop
    /// </summary>
    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (_textEditorCursorDisplay is null)
            return;

        if (KeyboardKeyFacts.IsMovementKey(keyboardEventArgs.Key))
        {
            _textEditorCursorDisplay.HandleKeyboardEvent(keyboardEventArgs);
        }
        else
        {
            Dispatcher.Dispatch(new TextEditorEditAction(
                TextEditorKey,
                new[] { (new ImmutableTextCursor(_cursor), _cursor) }.ToImmutableArray(),
                keyboardEventArgs,
                CancellationToken.None));

            if (_virtualize is not null)
                _virtualize.RefreshDataAsync();

            StateHasChanged();
        }
    }
    
    private async Task HandleOnCustomClick(CustomOnClick customOnClick)
    {
        if (_textEditorFontSize is null)
            return;
        
        var localTextEditorState = TextEditorStatesSelection.Value;

        if (_textEditorCursorDisplay is not null)
        {
            await _textEditorCursorDisplay.FocusAsync();
        }

        _mostRecentRelativeCoordinates = await JsRuntime
            .InvokeAsync<RelativeCoordinates>("blazorStudio.getRelativePosition",
                GetTextEditorElementId,
                customOnClick.ClientX,
                customOnClick.ClientY);

        var columnIndexDouble = _mostRecentRelativeCoordinates.RelativeX / _textEditorFontSize.CharacterWidth;
        var columnIndexRounded = Math.Round(columnIndexDouble, MidpointRounding.AwayFromZero);

        var rowIndex = new RowIndex((int)(_mostRecentRelativeCoordinates.RelativeY / _textEditorFontSize.RowHeight));

        if (rowIndex.Value >= localTextEditorState.LineEndingPositions.Length)
            rowIndex = new(localTextEditorState.LineEndingPositions.Length - 1);
        
        if (rowIndex.Value < 0)
            rowIndex.Value = 0;
        
        var columnIndex = new ColumnIndex((int)columnIndexRounded);

        var rowLength = TextEditorBase
            .GetLengthOfRow(rowIndex, localTextEditorState.LineEndingPositions);

        if (columnIndex.Value > rowLength)
            columnIndex = new(rowLength);

        if (columnIndex.Value < 0)
            columnIndex.Value = 0;
        
        var cursorIndexCoordinates = (rowIndex, columnIndex);

        _cursor.PreferredColumnIndex = columnIndex;
        _cursor.IndexCoordinates = cursorIndexCoordinates;
        
        _previousTextPartitionSequenceKey = SequenceKey.Empty();
    }

    private async ValueTask<ItemsProviderResult<TextCharacterSpan>> LoadTextCharacterSpans(
        ItemsProviderRequest request)
    {
        var localTextEditorState = TextEditorStatesSelection.Value;

        var numTextCharacterSpans = Math.Min(
                request.Count,
                localTextEditorState.LineEndingPositions.Length - request.StartIndex);

        _textPartition = localTextEditorState.GetTextPartition(new RectangularCoordinates(
            TopLeftCorner: (new(request.StartIndex), new(0)),
            BottomRightCorner: (new(request.StartIndex + numTextCharacterSpans), new(10))));

        return new ItemsProviderResult<TextCharacterSpan>(_textPartition.TextSpanRows,
            localTextEditorState.LineEndingPositions.Length);
    }

    private async Task ApplyRoslynSyntaxHighlightingAsyncOnClick()
    {
        // I don't want the IMMUTABLE state changing due to Blazor using a MUTABLE reference.
        var localTextEditor = TextEditorStatesSelection.Value;

        await localTextEditor.ApplyRoslynSyntaxHighlightingAsync();

        _previousTextPartitionSequenceKey = SequenceKey.Empty();
    }
    
    private int GetCursorPosition(TextCursor passedCursor)
    {
        var textEditor = TextEditorStatesSelection.Value;
        
        if (textEditor is null || !textEditor.LineEndingPositions.Any())
            return 0;
        
        var startOfTextSpanRowInclusive = passedCursor.IndexCoordinates.RowIndex.Value == 0
            ? 0
            : textEditor.LineEndingPositions[passedCursor.IndexCoordinates.RowIndex.Value - 1].positionIndex;

        return startOfTextSpanRowInclusive + passedCursor.IndexCoordinates.ColumnIndex.Value;
    }

    protected override void Dispose(bool disposing)
    {
        TextEditorOptionsStateWrap.StateChanged -= TextEditorOptionsStateWrapOnStateChanged;
        
        base.Dispose(disposing);
    }
}