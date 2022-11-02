using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public class TerminalStateReducer
{
    [ReducerMethod]
    public static TerminalState ReduceSetActiveTerminalCommandKeyAction(
        TerminalState inTerminalState,
        TerminalState.SetActiveTerminalCommandKeyAction setActiveTerminalCommandKeyAction)
    {
        return inTerminalState with
        {
            ActiveTerminalCommandKey = setActiveTerminalCommandKeyAction
                .TerminalCommandKey
        };
    }
}