using System.Diagnostics;
using System.Text;
using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Html;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.FooterWindowCase;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.RazorLib.ContextCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalState> TerminalStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private ContextBoundary _contextBoundary = null!;
    private ElementReference _terminalDisplayElementReference;
}