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
    
    private string CssStyleString => $"top: {TextCursor.RowIndex.Value * RowHeightInPixels}px;" +
                                     " " +
                                     $"left: {TextCursor.ColumnIndex.Value * CharacterWidthInPixels}px;";

    public void MoveCursor(KeyboardEventArgs keyboardEventArgs)
    {
        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT_KEY:
                if (TextCursor.ColumnIndex.Value > 0)
                {
                    TextCursor.ColumnIndex.Value--;
                }

                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY:
                if (TextCursor.RowIndex.Value < TextEditorBase.LineEndingPositions.Length - 1)
                {
                    TextCursor.ColumnIndex.Value = 0;
                    TextCursor.RowIndex.Value++;
                }
                
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_UP_KEY:
                if (TextCursor.RowIndex.Value > 0)
                {
                    TextCursor.ColumnIndex.Value = 0;
                    TextCursor.RowIndex.Value--;
                }

                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT_KEY:
                var startOfTextSpanRowInclusive = TextCursor.RowIndex.Value == 0
                    ? 0
                    : TextEditorBase.LineEndingPositions[TextCursor.RowIndex.Value - 1];

                var endOfTextSpanRowExclusive = TextEditorBase.LineEndingPositions[TextCursor.RowIndex.Value];

                var lengthOfTextSpanRow = endOfTextSpanRowExclusive - startOfTextSpanRowInclusive;
                
                if (TextCursor.ColumnIndex.Value < lengthOfTextSpanRow - 1)
                {
                    TextCursor.ColumnIndex.Value++;
                }

                break;
        }
        
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