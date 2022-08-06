using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using BlazorStudio.ClassLib.Store.StartupProject;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.ClassLib.Store.WorkspaceCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace BlazorStudio.RazorLib.StartupProjectControls;

public partial class StartupProjectControlsDisplay : FluxorComponent, IDisposable
{
    [Inject]
    private IState<StartupProjectState> StartupProjectStateWrap { get; set; } = null!;
    [Inject]
    private IState<SolutionState> SolutionStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private CancellationTokenSource _cancellationTokenSource = new();
    private bool _isEnqueuedToRun;
    private bool _isRunning;
    private EnqueueProcessOnTerminalEntryAction _enqueueProcessOnTerminalEntryAction;

    private void DispatchEnqueueProcessOnTerminalEntryAction()
    {
        var localStartupProjectState = StartupProjectStateWrap.Value;

        if (localStartupProjectState.ProjectAbsoluteFilePath is null)
            return;

        try
        {
            _isEnqueuedToRun = true;

            void OnStart()
            {
                _isEnqueuedToRun = false;

                _isRunning = true;
            }

            void OnEnd(Process finishedProcess)
            {
                _isRunning = false;

                InvokeAsync(StateHasChanged);
            }

            _enqueueProcessOnTerminalEntryAction = new EnqueueProcessOnTerminalEntryAction(
                TerminalStateFacts.ProgramTerminalEntry.TerminalEntryKey,
                $"dotnet run --project {StartupProjectStateWrap.Value.ProjectAbsoluteFilePath.GetAbsoluteFilePathString()}",
                null,
                OnStart,
                OnEnd,
                null,
                (_, _) => { },
                null,
                CancelTokenSourceAndGetNewToken());

            Dispatcher.Dispatch(_enqueueProcessOnTerminalEntryAction);
        }
        finally
        {
            _isEnqueuedToRun = false;
        }
    }

    public CancellationToken CancelTokenSourceAndGetNewToken()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new();

        return _cancellationTokenSource.Token;
    }

    protected override void Dispose(bool disposing)
    {
        _cancellationTokenSource?.Cancel();

        base.Dispose(disposing);
    }
}