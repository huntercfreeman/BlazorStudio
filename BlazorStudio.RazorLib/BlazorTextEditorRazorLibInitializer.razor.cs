using BlazorStudio.ClassLib.Store.TerminalCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib;

public partial class BlazorTextEditorRazorLibInitializer : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    protected override void OnInitialized()
    {
        foreach (var terminalSessionKey in TerminalSessionFacts.WELL_KNOWN_TERMINAL_SESSION_KEYS)
        {
            var terminalSession = new TerminalSession(
                null, 
                Dispatcher)
            {
                TerminalSessionKey = terminalSessionKey
            };
         
            Dispatcher.Dispatch(new TerminalSessionsReducer.RegisterTerminalSessionAction(
                terminalSession));
        }
        
        base.OnInitialized();
    }
}