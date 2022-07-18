using Fluxor;

namespace BlazorStudio.ClassLib.Store.EditorCase;

public class EditorStateReducer
{
    [ReducerMethod]
    public static EditorState ReduceSetOpenedAbsoluteFilePathAction(EditorState previousEditorState,
        SetOpenedAbsoluteFilePathAction setOpenedAbsoluteFilePathAction)
    {
        return new EditorState(setOpenedAbsoluteFilePathAction.AbsoluteFilePath);
    }
}