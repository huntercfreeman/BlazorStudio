using Fluxor;

namespace BlazorStudio.ClassLib.Store.MenuCase;

public class MenuStateReducer
{
    [ReducerMethod]
    public static MenuState ReduceRegisterMenuOptionAction(MenuState previousMenuState,
        RegisterMenuOptionAction registerMenuOptionAction)
    {
        return new MenuState(
            previousMenuState.MenuRecords
                .Add(registerMenuOptionAction.MenuOptionRecord));
    }

    [ReducerMethod]
    public static MenuState ReduceDisposeMenuOptionAction(MenuState previousMenuState,
        DisposeMenuOptionAction disposeMenuOptionAction)
    {
        return new MenuState(
            previousMenuState.MenuRecords
                .Remove(disposeMenuOptionAction.MenuOptionRecord));
    }
}