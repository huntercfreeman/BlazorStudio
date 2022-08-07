using BlazorStudio.ClassLib.Store.TerminalCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalSettingsDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalSettingsState> TerminalSettingsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private void OnShowTerminalOnProcessStartedChanged(ChangeEventArgs changeEventArgs)
    {
        var value = bool.Parse(changeEventArgs.Value?.ToString() ?? "true");

        var localTerminalSettingsState = TerminalSettingsStateWrap.Value;

        Dispatcher.Dispatch(new SetTerminalSettingsStateAction(localTerminalSettingsState with
        {
            ShowTerminalOnProcessStarted = value
        }));
    }
}