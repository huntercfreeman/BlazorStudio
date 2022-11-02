using System.Collections.Immutable;
using Fluxor;
using Microsoft.AspNetCore.Authorization;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

[FeatureState]
public record TerminalSessionsState
{
    public ImmutableDictionary<TerminalSessionKey, TerminalSession> TerminalSessionMap { get; }
    public IDispatcher Dispatcher { get; }

    public TerminalSessionsState(
        ImmutableDictionary<TerminalSessionKey, TerminalSession> terminalSessionMap,
        IDispatcher dispatcher)
    {
        TerminalSessionMap = terminalSessionMap;
        Dispatcher = dispatcher;
        
        foreach (var terminalSessionKey in TerminalSessionFacts.WELL_KNOWN_TERMINAL_SESSION_KEYS)
        {
            var terminalSession = new TerminalSession(
                null,
                dispatcher)
            {
                 TerminalSessionKey = terminalSessionKey
            };
            
            TerminalSessionMap = TerminalSessionMap
                .Add(terminalSessionKey, terminalSession);
        }
    }
}

