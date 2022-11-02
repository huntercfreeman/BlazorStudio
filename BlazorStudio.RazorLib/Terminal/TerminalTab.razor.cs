using BlazorStudio.ClassLib.Store.TerminalCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalTab : FluxorComponent
{
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public TerminalCommandKey TerminalCommandKey { get; set; } = null!;

    private string CssClassString => 
        $"bstudio_terminal-tab {ActiveTerminalCommandKeyCssClassString}";
    
    private string ActiveTerminalCommandKeyCssClassString => 
        IsActiveTerminalCommandKey
            ? "bstudio_active"
            : string.Empty;

    private bool IsActiveTerminalCommandKey => 
        TerminalStateWrap.Value.ActiveTerminalCommandKey == TerminalCommandKey;

    private Task DispatchSetActiveTerminalCommandKeyActionOnClick()
    {
        Dispatcher.Dispatch(
            new TerminalState.SetActiveTerminalCommandKeyAction(
                TerminalCommandKey));

        return Task.CompletedTask;
    }
}