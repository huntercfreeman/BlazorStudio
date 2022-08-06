using System.Collections.Immutable;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

[FeatureState]
public record TerminalState(int ActiveIndex, ImmutableList<TerminalEntry> TerminalEntries)
{
    public TerminalState() : this(0, ImmutableList<TerminalEntry>.Empty)
    {
        TerminalEntries = TerminalEntries.Add(TerminalStateFacts.ExecutionTerminalEntry);
        TerminalEntries = TerminalEntries.Add(TerminalStateFacts.GeneralTerminalEntry);
    }
}