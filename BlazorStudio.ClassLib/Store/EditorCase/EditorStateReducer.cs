using Fluxor;

namespace BlazorStudio.ClassLib.Store.EditorCase;

public class EditorStateReducer
{
    [ReducerMethod]
    public static EditorState ReduceSetActiveTextEditorKeyAction(EditorState previousEditorState,
        SetActiveTextEditorKeyAction setActiveTextEditorKeyAction)
    {
        return new EditorState(setActiveTextEditorKeyAction.TextEditorKey);
    }
}