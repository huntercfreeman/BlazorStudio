using System.Collections.Immutable;
using Fluxor;
using Microsoft.AspNetCore.Authorization;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

[FeatureState]
public record TerminalSessionsState(ImmutableDictionary<TerminalSessionKey, TerminalSession> terminalSessionMap)
{
    public ImmutableDictionary<TerminalSessionKey, TerminalSession> TerminalSessionMap { get; }
    public IDispatcher Dispatcher { get; }

    public TerminalSessionsState() 
        : this(ImmutableDictionary<TerminalSessionKey, TerminalSession>.Empty)
    {
        TerminalSessionMap = terminalSessionMap;
        
        foreach (var terminalSessionKey in TerminalSessionFacts.WELL_KNOWN_TERMINAL_SESSION_KEYS)
        {
            var terminalSession = new TerminalSession(
                null)
            {
                 TerminalSessionKey = terminalSessionKey
            };
            
            TerminalSessionMap = TerminalSessionMap
                .Add(terminalSessionKey, terminalSession);
        }
    }
}

