using System.Collections.Concurrent;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.FileSystemCase;
using BlazorStudio.ClassLib.TextEditor;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TextEditorCase;

public class TextEditorStatesEffects
{
    private readonly IState<TextEditorStates> _textEditorStatesWrap;
    private readonly IState<EditorState> _editorState;

    public TextEditorStatesEffects(IState<TextEditorStates> textEditorStatesWrap,
        IState<EditorState> editorState)
    {
        _textEditorStatesWrap = textEditorStatesWrap;
        _editorState = editorState;
    }
    
    [EffectMethod]
    public async Task HandleRequestSaveTextEditorAction(RequestSaveTextEditorAction requestSaveTextEditorAction,
        IDispatcher dispatcher)
    {
        var saveTarget = _textEditorStatesWrap.Value.TextEditors
            .FirstOrDefault(x =>
                x.TextEditorKey == (requestSaveTextEditorAction.TextEditorKey ?? _editorState.Value.TextEditorKey));

        if (saveTarget is null)
            return;

        var contentToWriteOut = saveTarget.GetAllText();
        
        dispatcher.Dispatch(new WriteToFileSystemAction(saveTarget.AbsoluteFilePath, contentToWriteOut));
        
        saveTarget.ClearEditBlocks();
    }
}