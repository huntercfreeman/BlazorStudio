using System.Collections.Immutable;
using Fluxor;
using Microsoft.AspNetCore.Authorization;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

[FeatureState]
public record TerminalSessionsState(ImmutableDictionary<TerminalSessionKey, TerminalSession> TerminalSessionMap)
{
    public TerminalSessionsState()
        : this(ImmutableDictionary<TerminalSessionKey, TerminalSession>.Empty)
    {
    }
}