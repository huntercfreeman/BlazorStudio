using Fluxor;

namespace BlazorStudio.ClassLib.Store.TextEditorResourceCase;

public class TextEditorResourceStateReducer
{
    [ReducerMethod]
    public static TextEditorResourceState ReduceSetTextEditorResourceStateAction(
        TextEditorResourceState inTextEditorResourceState,
        SetTextEditorResourceStateAction setTextEditorResourceStateAction)
    {
        if (inTextEditorResourceState.ResourceMap
            .ContainsKey(setTextEditorResourceStateAction.TextEditorKey))
        {
            return inTextEditorResourceState;
        }

        var nextMap =
            inTextEditorResourceState.ResourceMap
                .Add(
                    setTextEditorResourceStateAction.TextEditorKey,
                    setTextEditorResourceStateAction.AbsoluteFilePath);

        return inTextEditorResourceState with
        {
            ResourceMap = nextMap
        };
    }
}