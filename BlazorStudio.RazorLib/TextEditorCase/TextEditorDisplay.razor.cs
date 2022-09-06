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
using BlazorStudio.RazorLib.VirtualizeComponents;
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
    private bool _shouldMeasureDimensions = true;
    private TextEditorFontSize? _textEditorFontSize;
    private int _fontSizeMeasurementMultiplier = 6; 
    private string _fontSizeMeasurementTestData = "abcdefghijklmnopqrstuvwxyz0123456789";
    private HtmlElementMeasuredDimensions _textEditorElementMeasuredDimensions;

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
        _shouldMeasureDimensions = true;
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

        if (_shouldMeasureDimensions)
        {
            _textEditorFontSize = await JsRuntime.InvokeAsync<TextEditorFontSize>(
                "blazorStudio.measureFontSizeByElementId", 
                GetMeasureFontSizeElementId,
                _fontSizeMeasurementTestData.Length * _fontSizeMeasurementMultiplier);
            
            _textEditorElementMeasuredDimensions = await JsRuntime.InvokeAsync<HtmlElementMeasuredDimensions>(
                "blazorStudio.measureDimensionsByElementId", 
                GetTextEditorElementId);

            _shouldMeasureDimensions = false;
            
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

        var parameterToCountTabs = columnIndex.Value > rowLength
            ? rowLength
            : columnIndex.Value;
        
        // Tab keys for example can have a width > 1 character and must offset the cursor position accordingly.
        var tabsOnSameRowBeforeCursor = localTextEditorState
            .GetTabsCountOnSameRowBeforeCursor(rowIndex, new(parameterToCountTabs));
            
        var offsetTabsColumn = tabsOnSameRowBeforeCursor * (TextEditorBase.TabWidth - 1);

        columnIndex.Value -= offsetTabsColumn;
        
        var mostDigitsInALineNumber = localTextEditorState.LineEndingPositions.Length
            .ToString().Length;
        
        columnIndex.Value -= mostDigitsInALineNumber;
        
        columnIndex.Value -= TextEditorBase.FileContentMarginLeftSeparateFromLineNumbers;
        
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
    
    private VirtualizeCoordinateSystemResult<TextCharacterSpan> ItemsProviderFunc(VirtualizeCoordinateSystemScrollPosition scrollPosition)
    {
        var localTextEditorState = TextEditorStatesSelection.Value;
        var localTextEditorFontSize = _textEditorFontSize;

        if (localTextEditorFontSize is null)
            throw new ApplicationException($"{nameof(localTextEditorFontSize)} was null.");

        var startingIndex = (int)(scrollPosition.ScrollTop / localTextEditorFontSize.RowHeight);
        
        var count = (int)(Math.Ceiling(_textEditorElementMeasuredDimensions.Height / localTextEditorFontSize.RowHeight));
        
        var numTextCharacterSpans = Math.Min(
            count,
            localTextEditorState.LineEndingPositions.Length - startingIndex);

        _textPartition = localTextEditorState.GetTextPartition(new RectangularCoordinates(
            TopLeftCorner: (new(startingIndex), new(0)),
            BottomRightCorner: (new(startingIndex + numTextCharacterSpans), new(10))));

        var totalHeight = localTextEditorState.LineEndingPositions.Length * localTextEditorFontSize.RowHeight;
        var totalWidth = 750d;

        var contentWidth = totalWidth;
        var contentHeight = _textPartition.TextSpanRows.Count * localTextEditorFontSize.RowHeight;
        var contentLeftOffset = 0;
        var contentTopOffset = startingIndex * localTextEditorFontSize.RowHeight;

        // Validate minimum dimensions
        {
            contentWidth = contentWidth > _textEditorElementMeasuredDimensions.Width
                ? contentWidth
                : _textEditorElementMeasuredDimensions.Width;
            
            contentHeight = contentHeight > _textEditorElementMeasuredDimensions.Height
                ? contentHeight
                : _textEditorElementMeasuredDimensions.Height;
        }
        
        var leftBoundaryDimension = new VirtualizeCoordinateSystemBoundaryDimensions
        {
            Width = contentLeftOffset,
            Height = totalHeight,
            Left = 0,
            Top = 0
        };
        
        var bottomBoundaryDimensions = new VirtualizeCoordinateSystemBoundaryDimensions
        {
            Width = totalWidth,
            Height = totalHeight - (contentTopOffset + contentHeight),
            Left = 0,
            Top = contentTopOffset + contentHeight 
        };
        
        var topBoundaryDimensions = new VirtualizeCoordinateSystemBoundaryDimensions
        {
            Width = contentWidth,
            Height = scrollPosition.ScrollTop,
            Left = 0,
            Top = 0
        };
        
        var rightBoundaryDimensions = new VirtualizeCoordinateSystemBoundaryDimensions
        {
            Width = totalWidth - (contentLeftOffset + contentWidth),
            Height = totalHeight,
            Left = contentLeftOffset + contentWidth,
            Top = 0
        };

        var itemsToRender = _textPartition.TextSpanRows
            .Select((span, index) =>
                new VirtualizeCoordinateSystemEntry<TextCharacterSpan>()
                {
                    Width = 750d,
                    Height = localTextEditorFontSize.RowHeight,
                    Left = 0,
                    Top = localTextEditorFontSize.RowHeight * index + startingIndex,
                    Index = index + startingIndex,
                    Item = span
                })
            .ToImmutableArray();
        
        return new VirtualizeCoordinateSystemResult<TextCharacterSpan>()
        {
            ItemsToRender = itemsToRender, 
            LeftBoundaryDimensions = leftBoundaryDimension,
            BottomBoundaryDimensions = bottomBoundaryDimensions,
            TopBoundaryDimensions = topBoundaryDimensions,
            RightBoundaryDimensions = rightBoundaryDimensions 
        };
    }

    private async Task ApplyRoslynSyntaxHighlightingAsyncOnClick()
    {
        // I don't want the IMMUTABLE state changing due to Blazor using a MUTABLE reference.
        var localTextEditor = TextEditorStatesSelection.Value;

        await localTextEditor.ApplyRoslynSyntaxHighlightingAsync();

        _previousTextPartitionSequenceKey = SequenceKey.Empty();
    }

    private string GetTextSpanDimensions(
        TextCharacterSpan textSpan, 
        TextEditorFontSize textEditorFontSize,
        TextEditorBase localTextEditorState)
    {
        var mostDigitsInALineNumber = localTextEditorState.LineEndingPositions.Length
            .ToString().Length;

        var marginLeft = TextEditorBase.FileContentMarginLeftSeparateFromLineNumbers * textEditorFontSize.CharacterWidth;

        var lineNumbersWidth = (mostDigitsInALineNumber) * textEditorFontSize.CharacterWidth;
        
        var heightInPixels = textEditorFontSize.RowHeight;
        
        var topInPixels = textSpan.RowIndex * textEditorFontSize.RowHeight;

        return $"min-width: calc(100% - {lineNumbersWidth}px); height: {heightInPixels}px; left: {lineNumbersWidth + marginLeft}px; top: {topInPixels}px;";
    }

    private string GetLineNumberDimensions(
        TextCharacterSpan textSpan,
        TextEditorFontSize textEditorFontSize,
        TextEditorBase localTextEditorState)
    {
        var mostDigitsInALineNumber = localTextEditorState.LineEndingPositions.Length
            .ToString().Length;

        var padding = TextEditorBase.FileContentMarginLeftSeparateFromLineNumbers * textEditorFontSize.CharacterWidth;
        
        var paddingLeft = padding * .25;
        var paddingRight = padding * .75;

        var width = (mostDigitsInALineNumber * textEditorFontSize.CharacterWidth) 
                    + paddingRight;
        
        var heightInPixels = textEditorFontSize.RowHeight;
        
        var topInPixels = textSpan.RowIndex * textEditorFontSize.RowHeight;

        return $"padding-left: {paddingLeft}px; padding-right: {paddingRight}px; width: {width}px; height: {heightInPixels}px; left: 0; top: {topInPixels}px;";
    }
    
    protected override void Dispose(bool disposing)
    {
        TextEditorOptionsStateWrap.StateChanged -= TextEditorOptionsStateWrapOnStateChanged;
        
        base.Dispose(disposing);
    }
}


