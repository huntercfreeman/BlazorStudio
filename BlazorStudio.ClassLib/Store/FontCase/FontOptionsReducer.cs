using Fluxor;

namespace BlazorStudio.ClassLib.Store.FontCase;

public class FontOptionsReducer
{
    [ReducerMethod]
    public static FontOptionsState ReduceSetFontOptionsStateAction(FontOptionsState previousFontOptionsState,
        SetFontOptionsStateAction setFontOptionsStateAction)
    {
        return new(setFontOptionsStateAction.FontSize);
    }
}