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

public class TextEditorTests : TextEditorTestsBase
{
    private TextEditorKey _textEditorKey = TextEditorKey.NewTextEditorKey();

    [Fact]
    public async Task INSERTION()
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
    
    [Fact]
    public async Task BACKSPACE_KEY()
    {
        var expectedContent = await FileSystemProvider.ReadFileAsync(HelloWorldInCAbsoluteFilePath);
                
        Dispatcher.Dispatch(
            new RequestConstructTextEditorAction(
                _textEditorKey,
                DiscardTextFileAbsoluteFilePath,
                string.Empty,
                (str, token) => Task.CompletedTask,
                () => null));

        var textEditor = TextEditorStatesWrap.Value.TextEditors
            .Single(x => x.TextEditorKey == _textEditorKey);
        
        var cursor = new TextCursor();

        var keyboardEventArgs = await GenerateKeyboardEventArgsFromFileAsync(HelloWorldInCAbsoluteFilePath);

        foreach (var keyboardEventArg in keyboardEventArgs)
        {
            Dispatcher.Dispatch(new TextEditorEditAction(
                _textEditorKey,
                new[] { (new ImmutableTextCursor(cursor), cursor) }.ToImmutableArray(),
                keyboardEventArg,
                CancellationToken.None));
        }

        var actual = textEditor.GetAllText();
        
        Assert.Equal(expectedContent, actual);
    }
    
    [Fact]
    public async Task DELETE_KEY()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task ARROW_LEFT_KEY()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task ARROW_DOWN_KEY()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task ARROW_UP_KEY()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task ARROW_RIGHT_KEY()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task HOME_KEY()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async Task END_KEY()
    {
        throw new NotImplementedException();
    }
}