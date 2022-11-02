using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public class WellKnownTerminalSessionsStateReducer
{
    [ReducerMethod]
    public static WellKnownTerminalSessionsState ReduceSetActiveTerminalCommandKeyAction(
        WellKnownTerminalSessionsState inWellKnownTerminalSessionsState,
        WellKnownTerminalSessionsState.SetActiveTerminalCommandKey setActiveTerminalCommandKeyAction)
    {
        return inWellKnownTerminalSessionsState with
        {
            ActiveTerminalCommandKey = setActiveTerminalCommandKeyAction
                .TerminalCommandKey
        };
    }
}