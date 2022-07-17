using Fluxor;

namespace BlazorStudio.ClassLib.Store.DropdownCase;

public class DropdownStateReducer
{
    [ReducerMethod]
    public static DropdownState ReduceAddActiveDropdownKeyAction(DropdownState previousDropdownState,
        AddActiveDropdownKeyAction addActiveDropdownKeyAction)
    {
        return new DropdownState(
            previousDropdownState.ActiveDropdownKeys
                .Add(addActiveDropdownKeyAction.DropdownKey));
    }

    [ReducerMethod]
    public static DropdownState ReduceRemoveActiveDropdownKeyAction(DropdownState previousDropdownState,
        RemoveActiveDropdownKeyAction removeActiveDropdownKeyAction)
    {
        return new DropdownState(
            previousDropdownState.ActiveDropdownKeys
                .Remove(removeActiveDropdownKeyAction.DropdownKey));
    }

    [ReducerMethod(typeof(ClearActiveDropdownKeysAction))]
    public static DropdownState ReduceClearActiveDropdownKeysAction(DropdownState previousDropdownState)
    {
        return new DropdownState(
            previousDropdownState.ActiveDropdownKeys
                .Clear());
    }
}