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
                string.Empty,
                (str, token) => Task.CompletedTask,
                () => null));

        var cursor = new TextCursor();
        
        var keyboardEventArgs = HelloWorldInC
            .Select(c => new KeyboardEventArgs { Key = c.ToString() })
            .ToList();
        
        foreach (var keyboardEventArg in keyboardEventArgs)
        {
            Dispatcher.Dispatch(new TextEditorEditAction(
                _textEditorKey,
                new[] { (new ImmutableTextCursor(cursor), cursor) }.ToImmutableArray(),
                keyboardEventArg,
                CancellationToken.None));
        }
    }
    
    [Theory]
    // Intent: Start of document move cursor left
    // Expectation: Does nothing
    [InlineData(0, 0, 0, false, 0, 0, 0)]
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
            Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT
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
    
    [Fact]
    public void ARROW_RIGHT_KEY()
    {
        throw new NotImplementedException();
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