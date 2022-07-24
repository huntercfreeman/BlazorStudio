using Fluxor;

namespace BlazorStudio.ClassLib.Store.EditorCase;

public class EditorStateReducer
{
    [ReducerMethod]
    public static EditorState ReduceSetActiveTabIndexAction(EditorState previousEditorState,
        SetActiveTabIndexAction setActiveTabIndexAction)
    {
        return new EditorState(setActiveTabIndexAction.TabIndex);
    }
}