using System.Collections.Immutable;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.TextEditor;
using BlazorStudio.ClassLib.TextEditor.Cursor;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.TextEditorCase;

public partial class TextEditorCursorDisplay : ComponentBase
{
    [Parameter]
    public TextCursor TextCursor { get; set; } = null!;

    [Parameter]
    public TextEditorBase TextEditorBase { get; set; } = null!;

    [Parameter]
    public double RowHeightInPixels { get; set; } = 39;

    [Parameter]
    public double CharacterWidthInPixels { get; set; } = 16;

    private ElementReference? _textAreaCursorDisplayElementReference;

    private string CssStyleString => $"top: {TextCursor.IndexCoordinates.RowIndex.Value * RowHeightInPixels}px;" +
                                     " " +
                                     $"left: {TextCursor.IndexCoordinates.ColumnIndex.Value * CharacterWidthInPixels}px;";

    public void MoveCursor(KeyboardEventArgs keyboardEventArgs)
    {
        // Do not mutate the TextCursor.IndexCoordinates until
        // method is finished to ensure both coordinates are updated at the same time.
        (RowIndex rowIndex, ColumnIndex columnIndex) localIndexCoordinates =
            (new RowIndex(TextCursor.IndexCoordinates.RowIndex),
                new ColumnIndex(TextCursor.IndexCoordinates.ColumnIndex));

        var localLineEndingPositions = TextEditorBase.LineEndingPositions;
        
        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT_KEY:
            {
                if (localIndexCoordinates.columnIndex.Value > 0)
                {
                    localIndexCoordinates.columnIndex.Value--;
                }
                else if (localIndexCoordinates.rowIndex.Value > 0)
                {
                    localIndexCoordinates.rowIndex.Value--;
                    
                    var lengthOfTextSpanRow = TextEditorBase
                        .GetLengthOfRow(localIndexCoordinates.rowIndex, localLineEndingPositions);
                    
                    localIndexCoordinates.columnIndex = new ColumnIndex(lengthOfTextSpanRow - 1);
                }
                
                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY:
            {
                if (localIndexCoordinates.rowIndex.Value < localLineEndingPositions.Length - 1)
                {
                    localIndexCoordinates.rowIndex.Value++;
                    localIndexCoordinates.columnIndex = new ColumnIndex(0);
                }

                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_UP_KEY:
            {
                if (localIndexCoordinates.rowIndex.Value > 0)
                {
                    localIndexCoordinates.rowIndex.Value--;
                    localIndexCoordinates.columnIndex = new ColumnIndex(0);
                }

                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT_KEY:
            {
                var lengthOfTextSpanRow = TextEditorBase
                    .GetLengthOfRow(localIndexCoordinates.rowIndex, localLineEndingPositions);
                
                if (localIndexCoordinates.columnIndex.Value < lengthOfTextSpanRow - 1)
                {
                    localIndexCoordinates.columnIndex.Value++;
                }
                else if (localIndexCoordinates.rowIndex.Value < localLineEndingPositions.Length - 1)
                {
                    localIndexCoordinates.columnIndex = new ColumnIndex(0);
                    localIndexCoordinates.rowIndex.Value++;
                }

                break;
            }
        }

        TextCursor.IndexCoordinates = localIndexCoordinates;
        
        StateHasChanged();
    }

    public async Task FocusAsync()
    {
        if (_textAreaCursorDisplayElementReference is not null)
        {
            await _textAreaCursorDisplayElementReference.Value.FocusAsync();
        }
    }
}