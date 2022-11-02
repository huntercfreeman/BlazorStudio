using System.Text;
using BlazorStudio.ClassLib.Html;
using BlazorStudio.ClassLib.Store.TerminalCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalSessionsState> TerminalStateWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
}