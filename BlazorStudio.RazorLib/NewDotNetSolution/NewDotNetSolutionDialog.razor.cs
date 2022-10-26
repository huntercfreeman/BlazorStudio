using System.Diagnostics;
using System.Text;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.SolutionExplorerCase;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.RazorLib.TreeViewCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.NewDotNetSolution;

public partial class NewDotNetSolutionDialog : ComponentBase
{
    private bool _disableExecuteButton;
    private bool _finishedCreatingSolution;

    private string _solutionName = string.Empty;
    private bool _startingCreatingSolution;
    private readonly string dotnetNewSlnCommand = "dotnet new sln";
    private IAbsoluteFilePath? InputFileDialogSelection;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private string SolutionName => string.IsNullOrWhiteSpace(_solutionName)
        ? "{enter solution name}"
        : _solutionName;

    private string AbsoluteFilePathString => GetAbsoluteFilePathString();

    private string InterpolatedCommand => $"{dotnetNewSlnCommand} -o {_solutionName}";

    private void InputFileDialogOnEnterKeyDownOverride(
        (IAbsoluteFilePath absoluteFilePath, Action toggleIsExpanded) tupleArgument)
    {
        if (_disableExecuteButton || _finishedCreatingSolution)
            return;

        if (tupleArgument.absoluteFilePath.IsDirectory)
        {
            InputFileDialogSelection = tupleArgument.absoluteFilePath;
            InvokeAsync(StateHasChanged);
        }
    }

    private void InputFileDialogChooseContextMenuOption(
        TreeViewContextMenuEventDto<IAbsoluteFilePath> treeViewContextMenuEventDto)
    {
        if (_disableExecuteButton || _finishedCreatingSolution)
            return;

        if (treeViewContextMenuEventDto.Item.IsDirectory)
        {
            InputFileDialogSelection = treeViewContextMenuEventDto.Item;
            InvokeAsync(StateHasChanged);
        }
    }

    private string GetAbsoluteFilePathString()
    {
        var builder = new StringBuilder();

        if (InputFileDialogSelection is null ||
            !InputFileDialogSelection.IsDirectory)
            builder.Append($"{{pick a directory}}{Path.DirectorySeparatorChar}");
        else
            builder.Append(InputFileDialogSelection.GetAbsoluteFilePathString());

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

            var createdSolutionContainingDirectory = new AbsoluteFilePathDotNet(
                InputFileDialogSelection.GetAbsoluteFilePathString() + _solutionName, true, null);

            var createdSolutionFile = new AbsoluteFilePathDotNet(
                createdSolutionContainingDirectory.GetAbsoluteFilePathString() + _solutionName + '.' +
                ExtensionNoPeriodFacts.DOT_NET_SOLUTION, false, null);

            Dispatcher.Dispatch(new SetFolderExplorerAction(createdSolutionContainingDirectory));
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
                data => output = data,
                CancellationToken.None));
    }
}