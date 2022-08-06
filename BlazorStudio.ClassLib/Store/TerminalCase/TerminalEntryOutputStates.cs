using System.Collections.Immutable;
using System.Text;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

[FeatureState]
public record TerminalEntryOutputStates(ImmutableDictionary<TerminalEntryKey, string> OutputMap)
{
    public TerminalEntryOutputStates() : this(ImmutableDictionary<TerminalEntryKey, string>.Empty)
    {

    }
}