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
    public double RowHeightInPixels { get; set; }
    [Parameter]
    public double CharacterWidthInPixels { get; set; }

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
        var localPreferredColumnIndex = TextCursor.PreferredColumnIndex;

        void UpdatePreferredColumnIndexAndIndexCoordinates(ColumnIndex columnIndex)
        {
            localPreferredColumnIndex = columnIndex;
            localIndexCoordinates.columnIndex = columnIndex;
        }
        
        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
            {
                if (localIndexCoordinates.columnIndex.Value > 0)
                {
                    if (keyboardEventArgs.CtrlKey)
                    {
                        var columnIndex = TextEditorBase
                            .ClosestNonMatchingCharacterOnSameRowColumnIndex( 
                                localIndexCoordinates.rowIndex, 
                                localIndexCoordinates.columnIndex,
                                true);

                        UpdatePreferredColumnIndexAndIndexCoordinates(columnIndex);
                    }
                    else
                    {
                        UpdatePreferredColumnIndexAndIndexCoordinates(
                            new ColumnIndex(localIndexCoordinates.columnIndex.Value - 1));
                    }
                }
                else if (localIndexCoordinates.rowIndex.Value > 0)
                {
                    localIndexCoordinates.rowIndex.Value--;
                
                    var lengthOfTextSpanRow = TextEditorBase
                        .GetLengthOfRow(localIndexCoordinates.rowIndex, localLineEndingPositions);
                
                    UpdatePreferredColumnIndexAndIndexCoordinates(
                        new ColumnIndex(lengthOfTextSpanRow - 1));
                }    
                
                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
            {
                if (keyboardEventArgs.CtrlKey)
                {
                    // TODO: CtrlKey
                }
                else
                {
                    if (localIndexCoordinates.rowIndex.Value < localLineEndingPositions.Length - 1)
                    {
                        localIndexCoordinates.rowIndex.Value++;
                    
                        var postMoveRowLength = TextEditorBase
                            .GetLengthOfRow(localIndexCoordinates.rowIndex, localLineEndingPositions);

                        if (localPreferredColumnIndex.Value >= postMoveRowLength - 1)
                        {
                            localIndexCoordinates.columnIndex = new ColumnIndex(postMoveRowLength - 1);
                        }
                        else
                        {
                            localIndexCoordinates.columnIndex = new ColumnIndex(localPreferredColumnIndex);
                        }
                    }
                }
                
                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
            {
                
                if (keyboardEventArgs.CtrlKey)
                {
                    // TODO: CtrlKey
                }
                else
                {
                    if (localIndexCoordinates.rowIndex.Value > 0)
                    {
                        localIndexCoordinates.rowIndex.Value--;
                    
                        var postMoveRowLength = TextEditorBase
                            .GetLengthOfRow(localIndexCoordinates.rowIndex, localLineEndingPositions);

                        if (localPreferredColumnIndex.Value >= postMoveRowLength - 1)
                        {
                            localIndexCoordinates.columnIndex = new ColumnIndex(postMoveRowLength - 1);
                        }
                        else
                        {
                            localIndexCoordinates.columnIndex = new ColumnIndex(localPreferredColumnIndex);
                        }
                    }
                }
                
                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
            {
                var lengthOfTextSpanRow = TextEditorBase
                    .GetLengthOfRow(localIndexCoordinates.rowIndex, localLineEndingPositions);
                
                if (localIndexCoordinates.columnIndex.Value < lengthOfTextSpanRow - 1)
                {
                    if (keyboardEventArgs.CtrlKey)
                    {
                        var columnIndex = TextEditorBase
                            .ClosestNonMatchingCharacterOnSameRowColumnIndex( 
                                localIndexCoordinates.rowIndex, 
                                localIndexCoordinates.columnIndex,
                                false);

                        UpdatePreferredColumnIndexAndIndexCoordinates(columnIndex);
                    }
                    else
                    {
                        UpdatePreferredColumnIndexAndIndexCoordinates(
                            new ColumnIndex(localIndexCoordinates.columnIndex.Value + 1));
                    }
                }
                else if (localIndexCoordinates.rowIndex.Value < localLineEndingPositions.Length - 1)
                {
                    UpdatePreferredColumnIndexAndIndexCoordinates(
                        new ColumnIndex(0));
                    
                    localIndexCoordinates.rowIndex.Value++;
                }
                
                break;
            }
            case KeyboardKeyFacts.MovementKeys.HOME:
            {
                if (keyboardEventArgs.CtrlKey)
                {
                    localIndexCoordinates.rowIndex = new(0);
                }
                
                UpdatePreferredColumnIndexAndIndexCoordinates(new(0));
                
                break;
            }
            case KeyboardKeyFacts.MovementKeys.END:
            {
                if (keyboardEventArgs.CtrlKey)
                {
                    localIndexCoordinates.rowIndex = new(localLineEndingPositions.Length - 1);
                }
                
                var lengthOfTextSpanRow = TextEditorBase
                    .GetLengthOfRow(localIndexCoordinates.rowIndex, localLineEndingPositions);
                
                UpdatePreferredColumnIndexAndIndexCoordinates(new(lengthOfTextSpanRow - 1));
                
                break;
            }
        }

        TextCursor.IndexCoordinates = localIndexCoordinates;
        TextCursor.PreferredColumnIndex = localPreferredColumnIndex;
        
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