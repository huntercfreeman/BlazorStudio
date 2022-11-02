using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

[FeatureState]
public record WellKnownTerminalSessionsState(TerminalCommandKey ActiveTerminalCommandKey)
{
    public WellKnownTerminalSessionsState() : this(TerminalCommandKey.Empty)
    {
    }

    public record SetActiveWellKnownTerminalCommandKey(TerminalCommandKey TerminalCommandKey);
}