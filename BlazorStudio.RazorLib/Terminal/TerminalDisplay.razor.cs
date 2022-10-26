using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.RazorLib.ContextCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalDisplay : FluxorComponent
{
    private ContextBoundary _contextBoundary = null!;
    private ElementReference _terminalDisplayElementReference;
    [Inject]
    private IState<TerminalState> TerminalStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
}