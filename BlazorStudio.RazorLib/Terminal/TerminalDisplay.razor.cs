using System.Diagnostics;
using System.Text;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Html;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalState> TerminalStatesWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;
}