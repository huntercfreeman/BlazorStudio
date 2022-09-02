using System.Collections.Immutable;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.TextEditorCase;
using BlazorStudio.ClassLib.TextEditor;
using BlazorStudio.ClassLib.TextEditor.Cursor;
using BlazorStudio.ClassLib.TextEditor.Enums;
using BlazorStudio.ClassLib.TextEditor.IndexWrappers;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.Tests;

public class TextEditorBaseMovementTests : TextEditorTestsBase
{
    private TextEditorKey _textEditorKey = TextEditorKey.NewTextEditorKey();

    public TextEditorBaseMovementTests()
    {
        Dispatcher.Dispatch(
            new RequestConstructTextEditorAction(
                _textEditorKey,
                DiscardTextFileAbsoluteFilePath,
                HelloWorldInC,
                (str, token) => Task.CompletedTask,
                () => null));
    }
    
    // Test data is a C hello world program as of this comment.
    [Theory]
    // start of document go left; do nothing
    [InlineData(0, 0, 0, false, 0, 0, 0)]
    [InlineData(0, 0, 0, true, 0, 0, 0)]
    // 1 character deep into the document go left; ends up at start of document
    [InlineData(0, 1, 1, false, 0, 0, 0)]
    [InlineData(0, 1, 1, true, 0, 0, 0)]
    // start of second row go left; ends up at the end of row index 0
    [InlineData(1, 0, 0, false, 0, 19, 19)]
    [InlineData(1, 0, 0, true, 0, 19, 19)]
    public void ARROW_LEFT_KEY(int startRowIndex, int startColumnIndex, int startPreferredColumnIndex, 
        bool useControlKey, 
        int endRowIndex, int endColumnIndex, int endPreferredColumnIndex)
    {
        var textEditor = TextEditorStatesWrap.Value.TextEditors
            .Single(x => x.TextEditorKey == _textEditorKey);
        
        var cursor = new TextCursor
        {
            IndexCoordinates = (new(startRowIndex), new(startColumnIndex)),
            PreferredColumnIndex = new(startPreferredColumnIndex),
            TextCursorKind = TextCursorKind.Beam
        };

        var keyboardEventArgs = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
            CtrlKey = useControlKey
        };
        
        TextCursor.MoveCursor(keyboardEventArgs, cursor, textEditor);

        var expectedCursor = new TextCursor
        {
            IndexCoordinates = (new(endRowIndex), new(endColumnIndex)),
            PreferredColumnIndex = new(endPreferredColumnIndex),
            TextCursorKind = TextCursorKind.Beam
        };
        
        Assert.Equal(expectedCursor, cursor);
    }
    
    [Fact]
    public void ARROW_DOWN_KEY()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void ARROW_UP_KEY()
    {
        throw new NotImplementedException();
    }
    
    // Test data is a C hello world program as of this comment.
    [Theory]
    // end of document go right; do nothing
    [InlineData(6, 102, 102, false, 6, 102, 102)]
    [InlineData(6, 102, 102, true, 6, 102, 102)]
    // 1 character away from end of the document; ends up at end of document
    [InlineData(6, 101, 101, false, 6, 102, 102)]
    [InlineData(6, 101, 101, true, 6, 102, 102)]
    // end of row index 0 go right; ends up at the start of row index 1
    [InlineData(0, 19, 19, false, 1, 0, 0)]
    [InlineData(0, 19, 19, true, 1, 0, 0)]
    public void ARROW_RIGHT_KEY(int startRowIndex, int startColumnIndex, int startPreferredColumnIndex, 
        bool useControlKey, 
        int endRowIndex, int endColumnIndex, int endPreferredColumnIndex)
    {
        var textEditor = TextEditorStatesWrap.Value.TextEditors
            .Single(x => x.TextEditorKey == _textEditorKey);
        
        var cursor = new TextCursor
        {
            IndexCoordinates = (new(startRowIndex), new(startColumnIndex)),
            PreferredColumnIndex = new(startPreferredColumnIndex),
            TextCursorKind = TextCursorKind.Beam
        };

        var keyboardEventArgs = new KeyboardEventArgs
        {
            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
            CtrlKey = useControlKey
        };
        
        TextCursor.MoveCursor(keyboardEventArgs, cursor, textEditor);

        var expectedCursor = new TextCursor
        {
            IndexCoordinates = (new(endRowIndex), new(endColumnIndex)),
            PreferredColumnIndex = new(endPreferredColumnIndex),
            TextCursorKind = TextCursorKind.Beam
        };
        
        Assert.Equal(expectedCursor, cursor);
    }
    
    [Fact]
    public void HOME_KEY()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public void END_KEY()
    {
        throw new NotImplementedException();
    }
}