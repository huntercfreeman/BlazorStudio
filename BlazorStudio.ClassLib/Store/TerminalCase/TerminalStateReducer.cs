using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public class TerminalStateReducer
{
    [ReducerMethod]
    public static TerminalState ReduceSetActiveTerminalEntryAction(TerminalState previousTerminalState,
        SetActiveTerminalEntryAction setActiveTerminalEntryAction)
    {
        return previousTerminalState with
        {
            ActiveIndex = setActiveTerminalEntryAction.Index
        };
    }
}