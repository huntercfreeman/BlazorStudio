using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public class WellKnownTerminalSessionsStateReducer
{
    [ReducerMethod]
    public static WellKnownTerminalSessionsState ReduceSetActiveTerminalCommandKeyAction(
        WellKnownTerminalSessionsState inWellKnownTerminalSessionsState,
        WellKnownTerminalSessionsState.SetActiveWellKnownTerminalCommandKey setActiveWellKnownTerminalCommandKeyAction)
    {
        return inWellKnownTerminalSessionsState with
        {
            ActiveTerminalCommandKey = setActiveWellKnownTerminalCommandKeyAction
                .TerminalCommandKey
        };
    }
}