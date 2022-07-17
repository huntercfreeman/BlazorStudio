using Fluxor;

namespace BlazorStudio.ClassLib.Store.DropdownCase;

public class DropdownStateReducer
{
    [ReducerMethod]
    public static DropdownState ReduceSetActiveDropdownKeyAction(DropdownState previousDropdownState,
        SetActiveDropdownKeyAction setActiveDropdownKeyAction)
    {
        return new DropdownState(setActiveDropdownKeyAction.DropdownKey);
    }
}