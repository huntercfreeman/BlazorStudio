using System.Collections.Immutable;
using BlazorStudio.ClassLib.State;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

[FeatureState]
public record TerminalSessionWasModifiedState(ImmutableDictionary<TerminalSessionKey, StateKey> TerminalSessionWasModifiedMap)
{
    public TerminalSessionWasModifiedState()
        : this(ImmutableDictionary<TerminalSessionKey, StateKey>.Empty)
    {
    }
}