using System.Text;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.NewDotNetSolution;

public partial class NewDotNetSolutionDialog : ComponentBase
{
    private string _solutionName = string.Empty;
    private bool _disableExecuteButton;
    private bool _finishedCreatingProject;
    private IAbsoluteFilePath? InputFileDialogSelection;

    private string SolutionName => string.IsNullOrWhiteSpace(_solutionName)
        ? "{enter solution name}"
        : _solutionName;

    private string AbsoluteFilePathString => GetAbsoluteFilePathString();

    private void InputFileDialogOnEnterKeyDownOverride((IAbsoluteFilePath absoluteFilePath, Action toggleIsExpanded) tupleArgument)
    {
        if (_disableExecuteButton || _finishedCreatingProject)
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

        builder.Append(SolutionName);

        return builder.ToString();
    }
}