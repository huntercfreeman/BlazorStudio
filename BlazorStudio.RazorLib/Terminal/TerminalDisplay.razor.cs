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
    private IStateSelection<TerminalSessionsState, TerminalSession> TerminalSessionsStateSelection { get; set; } = null!;

    /// <summary>
    /// <see cref="TerminalSessionKey"/> is used to narrow down the terminal
    /// session.
    /// </summary>
    [Parameter, EditorRequired]
    public TerminalSessionKey TerminalSessionKey { get; set; } = null!;
    /// <summary>
    /// <see cref="TerminalCommandKey"/> is used to narrow down even further
    /// to the output of a specific command that was executed in a specific
    /// terminal session.
    /// <br/><br/>
    /// Optional
    /// </summary>
    [Parameter]
    public TerminalCommandKey? TerminalCommandKey { get; set; }

    protected override void OnInitialized()
    {
        TerminalSessionsStateSelection
            .Select(x => 
                x.TerminalSessionMap[TerminalSessionKey]);
        
        base.OnInitialized();
    }
}