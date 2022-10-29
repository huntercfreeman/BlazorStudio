using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.EditorCase;

[FeatureState]
public record EditorState(TextEditorKey ActiveTextEditorKey)
{
    public EditorState() : this(TextEditorKey.Empty)
    {
        
    }

    public record SetActiveTextEditorKeyAction(TextEditorKey TextEditorKey);

    public class EditorStateReducer
    {
        [ReducerMethod]
        public static EditorState ReduceSetActiveTextEditorKeyAction(
            EditorState inEditorState,
            SetActiveTextEditorKeyAction setActiveTextEditorKeyAction)
        {
            return inEditorState with
            {
                ActiveTextEditorKey = setActiveTextEditorKeyAction.TextEditorKey
            };
        }
    }
}