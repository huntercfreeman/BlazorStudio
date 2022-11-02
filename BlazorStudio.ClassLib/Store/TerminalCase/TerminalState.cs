using Fluxor;
using Microsoft.AspNetCore.Authorization;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

[FeatureState]
public record TerminalState(TerminalCommandKey ActiveTerminalCommandKey)
{
    public TerminalState() : this(TerminalCommandFacts.GENERAL_TERMINAL)
    {
        
    }
    
    public record SetActiveTerminalCommandKeyAction(TerminalCommandKey TerminalCommandKey);
}