using System.Diagnostics;
using System.Text;
using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Html;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.ContextCase;
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
    private IStateSelection<ContextState, ContextRecord> ContextStateSelector { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;
    
    private ContextBoundary _contextBoundary = null!;
    private ElementReference _terminalDisplayElementReference;
    
    protected override void OnInitialized()
    {
        ContextStateSelector
            .Select(x => x.ContextRecords[ContextFacts.TerminalDisplayContext.ContextKey]);

        base.OnInitialized();
    }
    
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            ContextStateSelector.Value.OnFocusRequestedEventHandler += ValueOnOnFocusRequestedEventHandler;
        }
        
        base.OnAfterRender(firstRender);
    }

    private async void ValueOnOnFocusRequestedEventHandler(object? sender, EventArgs e)
    {
        await _terminalDisplayElementReference.FocusAsync();
    }
    
    protected override void Dispose(bool disposing)
    {
        ContextStateSelector.Value.OnFocusRequestedEventHandler -= ValueOnOnFocusRequestedEventHandler;

        base.Dispose(disposing);
    }
}