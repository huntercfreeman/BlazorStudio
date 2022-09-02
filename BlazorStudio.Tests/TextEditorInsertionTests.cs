using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.TextEditorCase;
using BlazorStudio.ClassLib.TextEditor;
using BlazorStudio.ClassLib.TextEditor.Cursor;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.Tests;

public class TextEditorInsertionTests : TextEditorTestsBase
{
    private TextEditorKey _textEditorKey = TextEditorKey.NewTextEditorKey();

    [Fact]
    public void INSERTION()
    {
        var expectedContent = HelloWorldInC;
                
        Dispatcher.Dispatch(
            new RequestConstructTextEditorAction(
                _textEditorKey,
                DiscardTextFileAbsoluteFilePath,
                string.Empty,
                (str, token) => Task.CompletedTask,
                () => null));

        // Perform Test
        {
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
        
        var textEditor = TextEditorStatesWrap.Value.TextEditors
            .Single(x => x.TextEditorKey == _textEditorKey);
        
        var actual = textEditor.GetAllText();
        
        Assert.Equal(expectedContent, actual);
    }
}