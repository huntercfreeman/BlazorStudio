using Fluxor;

namespace BlazorStudio.ClassLib.Store.QuickSelectCase;

public class QuickSelectStateReducer
{
    [ReducerMethod]
    public QuickSelectState ReduceSetQuickSelectStateAction(QuickSelectState previousQuickSelectState,
        SetQuickSelectStateAction setQuickSelectStateAction)
    {
        return setQuickSelectStateAction.QuickSelectState;
    }
}