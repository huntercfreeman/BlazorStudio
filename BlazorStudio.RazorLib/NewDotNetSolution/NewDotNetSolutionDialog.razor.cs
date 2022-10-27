using System.Diagnostics;
using System.Text;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.SolutionExplorerCase;
using BlazorStudio.ClassLib.Store.TerminalCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.NewDotNetSolution;

public partial class NewDotNetSolutionDialog : ComponentBase
{
    private string _solutionName = string.Empty;
    private bool _startingCreatingSolution;
    private readonly string _dotnetNewSlnCommand = "dotnet new sln";
    private IAbsoluteFilePath? _inputFileDialogSelection;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private string SolutionName => string.IsNullOrWhiteSpace(_solutionName)
        ? "{enter solution name}"
        : _solutionName;

    private string AbsoluteFilePathString => GetAbsoluteFilePathString();

    private string InterpolatedCommand => $"{_dotnetNewSlnCommand} -o {_solutionName}";

    private void InputFileDialogOnEnterKeyDownOverride(
        (IAbsoluteFilePath absoluteFilePath, Action toggleIsExpanded) tupleArgument)
    {
        if (tupleArgument.absoluteFilePath.IsDirectory)
        {
            _inputFileDialogSelection = tupleArgument.absoluteFilePath;
            InvokeAsync(StateHasChanged);
        }
    }

    private void InputFileDialogChooseContextMenuOption(
        TreeViewContextMenuEventDto<IAbsoluteFilePath> treeViewContextMenuEventDto)
    {
        if (treeViewContextMenuEventDto.Item.IsDirectory)
        {
            _inputFileDialogSelection = treeViewContextMenuEventDto.Item;
            InvokeAsync(StateHasChanged);
        }
    }

    private string GetAbsoluteFilePathString()
    {
        var builder = new StringBuilder();

        if (_inputFileDialogSelection is null ||
            !_inputFileDialogSelection.IsDirectory)
            builder.Append($"{{pick a directory}}{Path.DirectorySeparatorChar}");
        else
            builder.Append(_inputFileDialogSelection.GetAbsoluteFilePathString());

        return builder.ToString();
    }

    private void DispatchTerminalNewSolutionOnClick()
    {
        if (_inputFileDialogSelection is null)
            return;
        
        void OnStart()
        {
            _startingCreatingSolution = true;
            InvokeAsync(StateHasChanged);
        }

        // Perhaps a bit peculiar to do this closure behavior...
        var output = string.Empty;

        void OnEnd(Process finishedProcess)
        {
            if (output == string.Empty)
                return;

            _startingCreatingSolution = false;

            InvokeAsync(StateHasChanged);

            var createdSolutionContainingDirectory = new AbsoluteFilePathDotNet(
                _inputFileDialogSelection.GetAbsoluteFilePathString() + _solutionName, true, null);

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
                _inputFileDialogSelection,
                OnStart,
                OnEnd,
                null,
                null,
                data => output = data,
                CancellationToken.None));
    }
}