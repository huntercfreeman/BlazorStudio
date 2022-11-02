using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public class TerminalStateReducer
{
    [ReducerMethod]
    public static TerminalSessionsState ReduceSetActiveTerminalCommandKeyAction(
        TerminalSessionsState inTerminalSessionsState,
        TerminalSessionsState.SetActiveTerminalCommandKeyAction setActiveTerminalCommandKeyAction)
    {
        return inTerminalSessionsState with
        {
            ActiveTerminalCommandKey = setActiveTerminalCommandKeyAction
                .TerminalCommandKey
        };
    }
}