using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public class TerminalEntryOutputReducer
{
    [ReducerMethod]
    public static TerminalEntryOutputStates ReduceSetTerminalEntryOutputStatesAction(
        TerminalEntryOutputStates previousTerminalEntryOutputStates,
        SetTerminalEntryOutputStatesAction setTerminalEntryOutputStatesAction)
    {
        if (!previousTerminalEntryOutputStates.OutputMap.TryGetValue(
                setTerminalEntryOutputStatesAction.TerminalEntryKey,
                out var terminalEntryOutput))
            terminalEntryOutput = string.Empty;

        var nextOutputMap = previousTerminalEntryOutputStates.OutputMap
            .SetItem(setTerminalEntryOutputStatesAction.TerminalEntryKey,
                terminalEntryOutput + setTerminalEntryOutputStatesAction.Output);

        return new TerminalEntryOutputStates(nextOutputMap);
    }

    [ReducerMethod]
    public static TerminalEntryOutputStates ReduceClearTerminalEntryOutputStatesAction(
        TerminalEntryOutputStates previousTerminalEntryOutputStates,
        ClearTerminalEntryOutputStatesAction clearTerminalEntryOutputStatesAction)
    {
        var nextOutputMap = previousTerminalEntryOutputStates.OutputMap
            .SetItem(clearTerminalEntryOutputStatesAction.TerminalEntryKey,
                string.Empty);

        return new TerminalEntryOutputStates(nextOutputMap);
    }
}