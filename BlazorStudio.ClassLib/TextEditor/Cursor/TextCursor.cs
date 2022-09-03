using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.TextEditor.Enums;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.ClassLib.TextEditor.Cursor;

public class TextCursor
{
    /// <summary>
    /// When changing both <see cref="RowIndex"/> and <see cref="ColumnIndex"/>
    /// this property being a Tuple helps change coordinates as a 'transaction'.
    /// </summary>
    public (RowIndex RowIndex, ColumnIndex ColumnIndex) IndexCoordinates { get; set; }
        = (new(0), new(0));
    /// <summary>
    /// Store <see cref="ColumnIndex"/> after ArrowLeft or other movements that influence this.
    /// <br/><br/>
    /// When moving (ArrowUp or ArrowDown) one might come across a row that has a column length
    /// not long enough and <see cref="IndexCoordinates"/> will lower the <see cref="ColumnIndex"/>
    /// to an index within range.
    /// <br/><br/>
    /// If one then proceeds to find a row that WOULD have had a column length long enough.
    /// Then the <see cref="ColumnIndex"/> is restored using the stored <see cref="PreferredColumnIndex"/>.
    /// </summary>
    public ColumnIndex PreferredColumnIndex { get; set; } = new(0);
    public TextCursorKind TextCursorKind { get; set; } = TextCursorKind.Beam;
    
    public static void MoveCursor(KeyboardEventArgs keyboardEventArgs, TextCursor textCursor, TextEditorBase textEditor)
    {
        // Do not mutate the TextCursor.IndexCoordinates until
        // method is finished to ensure both coordinates are updated at the same time.
        (RowIndex rowIndex, ColumnIndex columnIndex) localIndexCoordinates =
            (new RowIndex(textCursor.IndexCoordinates.RowIndex),
                new ColumnIndex(textCursor.IndexCoordinates.ColumnIndex));

        var localLineEndingPositions = textEditor.LineEndingPositions;
        var localPreferredColumnIndex = textCursor.PreferredColumnIndex;

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
                        var columnIndex = textEditor
                            .ClosestNonMatchingCharacterOnSameRowColumnIndex( 
                                localIndexCoordinates.rowIndex, 
                                new ColumnIndex(localIndexCoordinates.columnIndex.Value),
                                true);
                        
                        if (columnIndex.Value != localIndexCoordinates.columnIndex.Value)
                            columnIndex.Value++;
                            
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
                        new ColumnIndex(lengthOfTextSpanRow));
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

                        localIndexCoordinates.columnIndex = localPreferredColumnIndex.Value > postMoveRowLength 
                            ? new ColumnIndex(postMoveRowLength) 
                            : new ColumnIndex(localPreferredColumnIndex);
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

                        localIndexCoordinates.columnIndex = localPreferredColumnIndex.Value > postMoveRowLength 
                            ? new ColumnIndex(postMoveRowLength) 
                            : new ColumnIndex(localPreferredColumnIndex);
                    }
                }
                
                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
            {
                var lengthOfTextSpanRow = TextEditorBase
                    .GetLengthOfRow(localIndexCoordinates.rowIndex, localLineEndingPositions);
                
                if (localIndexCoordinates.columnIndex.Value < lengthOfTextSpanRow)
                {
                    if (keyboardEventArgs.CtrlKey)
                    {
                        var columnIndex = textEditor
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
                
                UpdatePreferredColumnIndexAndIndexCoordinates(new(lengthOfTextSpanRow));
                
                break;
            }
        }

        textCursor.IndexCoordinates = localIndexCoordinates;
        textCursor.PreferredColumnIndex = localPreferredColumnIndex;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj.GetType() == GetType())
        {
            var otherTextCursor = (TextCursor)obj;

            return IndexCoordinates.RowIndex.Value == otherTextCursor.IndexCoordinates.RowIndex.Value &&
                   IndexCoordinates.ColumnIndex.Value == otherTextCursor.IndexCoordinates.ColumnIndex.Value &&
                   PreferredColumnIndex.Value == otherTextCursor.PreferredColumnIndex.Value &&
                   TextCursorKind == otherTextCursor.TextCursorKind;
        }

        return false;
    }
}