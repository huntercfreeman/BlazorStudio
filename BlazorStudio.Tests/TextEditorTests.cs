using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.TextEditorCase;
using BlazorStudio.ClassLib.TextEditor;
using BlazorStudio.ClassLib.TextEditor.Cursor;
using Fluxor;

namespace BlazorStudio.Tests;

public class TextEditorTests : ExperimentalTextEditorTestsBase
{
    private TextEditorKey _textEditorKey = TextEditorKey.NewTextEditorKey();
    private IAbsoluteFilePath _absoluteFilePath = new AbsoluteFilePath("/BlazorStudioTestGround/main.c", false);
    
    [Fact]
    public async Task TEST()
    {
        var content = await FileSystemProvider.ReadFileAsync(_absoluteFilePath);
                
        Dispatcher.Dispatch(
            new RequestConstructTextEditorAction(
                _textEditorKey,
                _absoluteFilePath,
                content,
                (str, token) => Task.CompletedTask,
                () => null));
        
        Dispatcher.Dispatch(new SetActiveTextEditorKeyAction(_textEditorKey));

        var cursor = new TextCursor();
        
        Dispatcher.Dispatch(new TextEditorEditAction(
            _textEditorKey,
            new[] { (new ImmutableTextCursor(_cursor), _cursor) }.ToImmutableArray(),
            keyboardEventArgs,
            CancellationToken.None));
    }
}