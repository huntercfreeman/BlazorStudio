using System.Diagnostics;
using System.Text;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.SolutionExplorerCase;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.ClassLib.Store.WorkspaceCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using static BlazorStudio.RazorLib.NewCSharpProject.NewCSharpProjectDialog;

namespace BlazorStudio.RazorLib.NewDotNetSolution;

public partial class NewDotNetSolutionDialog : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private string _solutionName = string.Empty;
    private bool _disableExecuteButton;
    private bool _finishedCreatingSolution;
    private bool _startingCreatingSolution;
    private IAbsoluteFilePath? InputFileDialogSelection;
    private string dotnetNewSlnCommand = "dotnet new sln";

    private string SolutionName => string.IsNullOrWhiteSpace(_solutionName)
        ? "{enter solution name}"
        : _solutionName;

    private string AbsoluteFilePathString => GetAbsoluteFilePathString();

    private string InterpolatedCommand => $"{dotnetNewSlnCommand} -o {_solutionName}";

    private void InputFileDialogOnEnterKeyDownOverride((IAbsoluteFilePath absoluteFilePath, Action toggleIsExpanded) tupleArgument)
    {
        if (_disableExecuteButton || _finishedCreatingSolution)
            return;

        if (tupleArgument.absoluteFilePath.IsDirectory)
        {
            InputFileDialogSelection = tupleArgument.absoluteFilePath;
            InvokeAsync(StateHasChanged);
        }
    }
    
    private string GetAbsoluteFilePathString()
    {
        var builder = new StringBuilder();

        if (InputFileDialogSelection is null || 
            !InputFileDialogSelection.IsDirectory)
        {
            builder.Append($"{{pick a directory}}{Path.DirectorySeparatorChar}");
        }
        else
        {
            builder.Append(InputFileDialogSelection.GetAbsoluteFilePathString());
        }

        return builder.ToString();
    }
    
    private void DispatchTerminalNewSolutionOnClick()
    {
        void OnStart()
        {
            _startingCreatingSolution = true;
            InvokeAsync(StateHasChanged);
        }

        // Perhaps a bit peculiar to do this closure behavior...
        var output = string.Empty;

        void OnEnd(Process finishedProcess)
        {
            if (output is null)
                return;

            _startingCreatingSolution = false;

            InvokeAsync(StateHasChanged);

            var createdSolutionContainingDirectory = new AbsoluteFilePath(
                InputFileDialogSelection.GetAbsoluteFilePathString() + _solutionName, true);
            
            var createdSolutionFile = new AbsoluteFilePath(
                createdSolutionContainingDirectory.GetAbsoluteFilePathString() + _solutionName + '.' + ExtensionNoPeriodFacts.DOT_NET_SOLUTION, false);

            Dispatcher.Dispatch(new SetWorkspaceAction(createdSolutionContainingDirectory));
            Dispatcher.Dispatch(new SetSolutionExplorerAction(createdSolutionFile, SequenceKey.NewSequenceKey()));

            Dispatcher.Dispatch(new DisposeDialogAction(DialogRecord));
        }

        Dispatcher
            .Dispatch(new EnqueueProcessOnTerminalEntryAction(
                TerminalStateFacts.GeneralTerminalEntry.TerminalEntryKey,
                InterpolatedCommand,
                InputFileDialogSelection,
                OnStart,
                OnEnd,
                null,
                null,
                (data) => output = data,
                CancellationToken.None));
    }
}