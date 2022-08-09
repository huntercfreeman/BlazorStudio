using Fluxor;

namespace BlazorStudio.ClassLib.Store.IconCase;

public class IconOptionsReducer
{
    [ReducerMethod]
    public static IconOptionsState ReduceSetIconOptionsStateAction(IconOptionsState previousIconOptionsState,
        SetIconOptionsStateAction setIconOptionsStateAction)
    {
        return new(setIconOptionsStateAction.IconSize);
    }
}