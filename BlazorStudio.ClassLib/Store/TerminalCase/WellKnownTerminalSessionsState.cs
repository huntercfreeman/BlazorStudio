using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

[FeatureState]
public record WellKnownTerminalSessionsState(TerminalSessionKey ActiveTerminalSessionKey)
{
    public WellKnownTerminalSessionsState() : this(TerminalSessionKey.Empty)
    {
    }

    public record SetActiveWellKnownTerminalSessionKey(TerminalSessionKey TerminalCommandKey);
}