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
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

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
    private StringBuilder _outputBuilder;
    private EnqueueProcessOnTerminalEntryAction _enqueueProcessOnTerminalEntryAction;

    private void DispatchEnqueueProcessOnTerminalEntryAction()
    {
        _outputBuilder = new();

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
                //Failed to bind to address

                var output = _outputBuilder.ToString();

                if (output.Contains("System.IO.IOException: Failed to bind to address"))
                {
                    var z = 2;
                }

                _isRunning = false;

                InvokeAsync(StateHasChanged);
            }

            var containingDirectoryOfProject = (IAbsoluteFilePath) (StartupProjectStateWrap.Value.ProjectAbsoluteFilePath.Directories.Last());

            _enqueueProcessOnTerminalEntryAction = new EnqueueProcessOnTerminalEntryAction(
                TerminalStateFacts.ProgramTerminalEntry.TerminalEntryKey,
                $"dotnet run --project {StartupProjectStateWrap.Value.ProjectAbsoluteFilePath.GetAbsoluteFilePathString()}",
                containingDirectoryOfProject,
                OnStart,
                OnEnd,
                null,
                OutputDataReceived,
                null,
                CancelTokenSourceAndGetNewToken());

            Dispatcher.Dispatch(_enqueueProcessOnTerminalEntryAction);
        }
        finally
        {
            _isEnqueuedToRun = false;
        }
    }

    private void OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        _outputBuilder.Append(e.Data ?? string.Empty);
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

        if (_enqueueProcessOnTerminalEntryAction is not null)
        {
            _enqueueProcessOnTerminalEntryAction.InvokeKillRequestedEventHandler();
        }

        base.Dispose(disposing);
    }
}