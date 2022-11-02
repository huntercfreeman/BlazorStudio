using BlazorStudio.ClassLib.Store.TerminalCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalTab : FluxorComponent
{
    [Inject]
    private IState<WellKnownTerminalSessionsState> WellKnownTerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public TerminalSessionKey WellKnownTerminalSessionKey { get; set; } = null!;

    private string CssClassString => 
        $"bstudio_terminal-tab {ActiveTerminalCommandKeyCssClassString}";
    
    private string ActiveTerminalCommandKeyCssClassString => 
        IsActiveTerminalCommandKey
            ? "bstudio_active"
            : string.Empty;

    private bool IsActiveTerminalCommandKey => 
        WellKnownTerminalSessionsStateWrap.Value.ActiveTerminalSessionKey == 
        WellKnownTerminalSessionKey;

    private Task DispatchSetActiveTerminalCommandKeyActionOnClick()
    {
        Dispatcher.Dispatch(
            new WellKnownTerminalSessionsState.SetActiveWellKnownTerminalSessionKey(
                WellKnownTerminalSessionKey));

        return Task.CompletedTask;
    }
}