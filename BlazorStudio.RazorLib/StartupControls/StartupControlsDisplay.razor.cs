using BlazorStudio.ClassLib.Store.ProgramExecutionCase;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.ClassLib.Store.WorkspaceCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.StartupControls;

public partial class StartupControlsDisplay : FluxorComponent
{
    [Inject]
    private IState<WorkspaceState> WorkspaceStateWrap { get; set; } = null!;
    [Inject]
    private IState<ProgramExecutionState> ProgramExecutionStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    private Task StartProgramWithoutDebuggingOnClick()
    {
        var startProgramWithoutDebugging = new TerminalCommand(
            TerminalCommandKey.NewTerminalCommandKey(), 
            );
        
        Dispatcher.Dispatch(
            new TerminalStateEffects.QueueTerminalCommandToExecuteAction(
                ));
    }
}