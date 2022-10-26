using System.Diagnostics;
using System.Text;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.NotificationCase;
using BlazorStudio.ClassLib.Store.RoslynWorkspaceState;
using BlazorStudio.ClassLib.Store.SolutionCase;
using BlazorStudio.ClassLib.Store.StartupProject;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.RazorLib.ExceptionNotifications;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.StartupProjectControls;

public partial class StartupProjectControlsDisplay : FluxorComponent
{
    private CancellationTokenSource _cancellationTokenSource = new();
    private EnqueueProcessOnTerminalEntryAction _enqueueProcessOnTerminalEntryAction;
    private bool _isEnqueuedToRun;
    private bool _isRunning;
    private StringBuilder _outputBuilder;
    [Inject]
    private IState<StartupProjectState> StartupProjectStateWrap { get; set; } = null!;
    [Inject]
    private IState<SolutionState> SolutionStateWrap { get; set; } = null!;
    [Inject]
    private IState<RoslynWorkspaceState> RoslynWorkspaceState { get; set; } = null!;
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IState<ContextState> ContextStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private void DispatchEnqueueProcessOnTerminalEntryAction()
    {
        _outputBuilder = new StringBuilder();

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
                var output = _outputBuilder.ToString();

                if (output.Contains("System.IO.IOException: Failed to bind to address"))
                {
                    var notification = new NotificationRecord(NotificationKey.NewNotificationKey(),
                        "Detected: 'Failed to bind to address'",
                        typeof(FailedToBindNotification),
                        new Dictionary<string, object?>
                        {
                            {
                                nameof(FailedToBindNotification.ProjectAbsoluteFilePath),
                                localStartupProjectState.ProjectAbsoluteFilePath
                            },
                        });

                    Dispatcher.Dispatch(new RegisterNotificationAction(notification));
                }
                else if (output.Contains("\"ProcessFrameworkReferences\" task failed"))
                {
                    var notification = new NotificationRecord(NotificationKey.NewNotificationKey(),
                        "Detected: 'ProcessFrameworkReferences task failed'",
                        typeof(ProcessFrameworkReferencesTaskFailedNotification),
                        new Dictionary<string, object?>
                        {
                            {
                                nameof(ProcessFrameworkReferencesTaskFailedNotification.ProjectAbsoluteFilePath),
                                localStartupProjectState.ProjectAbsoluteFilePath
                            },
                        });

                    Dispatcher.Dispatch(new RegisterNotificationAction(notification));
                }

                _isRunning = false;

                InvokeAsync(StateHasChanged);
            }

            var containingDirectoryOfProject =
                (IAbsoluteFilePath)StartupProjectStateWrap.Value.ProjectAbsoluteFilePath.Directories.Last();

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
        _cancellationTokenSource = new CancellationTokenSource();

        return _cancellationTokenSource.Token;
    }

    protected override void Dispose(bool disposing)
    {
        _cancellationTokenSource?.Cancel();

        if (_enqueueProcessOnTerminalEntryAction is not null)
            _enqueueProcessOnTerminalEntryAction.InvokeKillRequestedEventHandler();

        base.Dispose(disposing);
    }
}