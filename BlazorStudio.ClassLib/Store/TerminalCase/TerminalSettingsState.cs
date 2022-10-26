using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

[FeatureState]
public record TerminalSettingsState(bool ShowTerminalOnProcessStarted)
{
    private TerminalSettingsState() : this(true)
    {
    }
}