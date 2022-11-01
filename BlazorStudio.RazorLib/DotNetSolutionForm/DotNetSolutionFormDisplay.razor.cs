using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using BlazorStudio.ClassLib.CommandLine;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.DotNetSolutionForm;

public partial class DotNetSolutionFormDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private string _solutionName = string.Empty;
    private IAbsoluteFilePath? _inputFileDialogSelection;

    private string SolutionName => string.IsNullOrWhiteSpace(_solutionName)
        ? "{enter solution name}"
        : _solutionName;

    private string SolutionDirectoryAbsoluteFilePathString => 
        GetAbsoluteFilePathString();

    private string InterpolatedCommand => 
        $"{DotNetCliFacts.DotnetNewSlnCommand} -o {_solutionName}";

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

    private void RequestInputFileForParentDirectory()
    {
        Dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                "Directory for new .NET Solution",
                afp =>
                {
                    _inputFileDialogSelection = afp;
                    InvokeAsync(StateHasChanged);

                    return Task.CompletedTask;
                },
                afp =>
                {
                    if (afp is null ||
                        !afp.IsDirectory)
                    {
                        return Task.FromResult(false);
                    }
                    
                    return Task.FromResult(true);
                },
                new []
                {
                    new InputFilePattern(
                        "Directory",
                        afp => afp.IsDirectory)
                }.ToImmutableArray()));
    }
}