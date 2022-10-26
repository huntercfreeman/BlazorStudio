using Fluxor;

namespace BlazorStudio.ClassLib.Store.ThemeCase;

public class ThemeStateReducer
{
    [ReducerMethod]
    public ThemeState ReduceSetThemeStateAction(ThemeState previousThemeState,
        SetThemeStateAction setThemeStateAction)
    {
        return new ThemeState(setThemeStateAction.ThemeKey);
    }
}