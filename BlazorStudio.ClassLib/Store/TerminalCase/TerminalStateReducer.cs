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
    
    [ReducerMethod]
    public static TerminalState ReduceSetTerminalEntryIsExecutingAction(TerminalState previousTerminalState,
        SetTerminalEntryIsExecutingAction setTerminalEntryIsExecutingAction)
    {
        var terminalEntry = previousTerminalState.TerminalEntries
            .FirstOrDefault(t => t.TerminalEntryKey == setTerminalEntryIsExecutingAction.TerminalEntryKey);

        if (terminalEntry is null)
            return previousTerminalState;

        var nextTerminalEntry = terminalEntry with
        {
            IsExecuting = setTerminalEntryIsExecutingAction.IsExecuting
        };

        var nextTerminalState = previousTerminalState with
        {
            TerminalEntries = previousTerminalState.TerminalEntries
                .Replace(terminalEntry, nextTerminalEntry)
        };

        return nextTerminalState;
    }
}