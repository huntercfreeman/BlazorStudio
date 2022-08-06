using System.Collections.Immutable;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record TerminalEntry(string Title);

public record SetActiveTerminalEntryAction(int Index);

[FeatureState]
public record TerminalState(int ActiveIndex, ImmutableList<TerminalEntry> TerminalEntries)
{
    public TerminalState() : this(0, ImmutableList<TerminalEntry>.Empty)
    {
        var executionTerminal = new TerminalEntry("Execution");
        var generalTerminal = new TerminalEntry("General");

        TerminalEntries = TerminalEntries.Add(executionTerminal);
        TerminalEntries = TerminalEntries.Add(generalTerminal);
    }
}

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
