using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public class TerminalSettingsReducer
{
    [ReducerMethod]
    public static TerminalSettingsState ReduceSetTerminalSettingsStateAction(TerminalSettingsState previousTerminalSettingsState,
        SetTerminalSettingsStateAction setTerminalSettingsStateAction)
    {
        return setTerminalSettingsStateAction.NextTerminalSettingsState;
    }
}