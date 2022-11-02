using System.Text;
using BlazorStudio.ClassLib.CommandLine;
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
        var programExecutionState = ProgramExecutionStateWrap.Value;

        if (programExecutionState.StartupProjectAbsoluteFilePath is null)
            return Task.CompletedTask;
        
        var startProgramWithoutDebugging = new TerminalCommand(
            TerminalSessionFacts.EXECUTION_TERMINAL,
            null,
            async terminalCommand =>
            {
                var terminalSession = await TerminalSession
                    .BeginSession(terminalCommand);
                
                await terminalSession.ExecuteCommand(
                    DotNetCliFacts
                        .FormatStartProjectWithoutDebugging(
                            programExecutionState.StartupProjectAbsoluteFilePath),
                    Dispatcher);
            },
            new StringBuilder(),
            new StringBuilder());
        
        Dispatcher.Dispatch(
            new TerminalStateEffects.QueueTerminalCommandToExecuteAction(
                startProgramWithoutDebugging));
        
        return Task.CompletedTask;
    }
}