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
    public async Task TEST()
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

        var keyboardKeyCodes = await GenerateKeyboardKeyCodesFromFileAsync(HelloWorldInCAbsoluteFilePath);

        foreach (var keyCode in keyboardKeyCodes)
        {
            var keyboardEventArg = new KeyboardEventArgs
            {
                Key = keyCode.IsLower ? keyCode.Key : keyCode.Key.ToUpperInvariant(),
                Code = keyCode.Code
            };
            
            Dispatcher.Dispatch(new TextEditorEditAction(
                _textEditorKey,
                new[] { (new ImmutableTextCursor(cursor), cursor) }.ToImmutableArray(),
                keyboardEventArg,
                CancellationToken.None));
        }

        var actual = textEditor.GetAllText();
        
        Assert.Equal(expectedContent, actual);
    }
}