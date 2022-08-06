using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.NewDotNetSolution;

public partial class NewDotNetSolutionDialog : ComponentBase
{
    private string _solutionName = string.Empty;
    private bool _disableExecuteButton;
    private bool _finishedCreatingProject;
    private IAbsoluteFilePath? InputFileDialogSelection;

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
}