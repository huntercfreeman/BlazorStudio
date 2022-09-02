using Fluxor;

namespace BlazorStudio.ClassLib.Store.TextEditorCase;

public class TextEditorOptionsReducer
{
    [ReducerMethod]
    public static TextEditorOptionsState ReduceSetTextEditorOptionsStateAction(TextEditorOptionsState previousTextEditorOptionsState,
        SetTextEditorOptionsAction setTextEditorOptionsAction)
    {
        return new(setTextEditorOptionsAction.FontSize);
    }
}