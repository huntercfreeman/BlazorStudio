using System.Collections.Immutable;
using BlazorStudio.ClassLib.ComponentRenderers;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.InputFile;
using BlazorStudio.ClassLib.TreeViewImplementations;

namespace BlazorStudio.ClassLib.Store.InputFileCase;

public partial record InputFileState
{
    public record RequestInputFileStateFormAction(string Message, Func<IAbsoluteFilePath?, Task> OnAfterSubmitFunc, Func<IAbsoluteFilePath?, Task<bool>> SelectionIsValidFunc, ImmutableArray<InputFilePattern> InputFilePatterns);
    public record SetSelectedTreeViewModelAction(TreeViewAbsoluteFilePath? SelectedTreeViewModel);
    public record SetOpenedTreeViewModelAction(TreeViewAbsoluteFilePath TreeViewModel, IBlazorStudioComponentRenderers BlazorStudioComponentRenderers, IFileSystemProvider FileSystemProvider, IEnvironmentProvider EnvironmentProvider);
    public record SetSelectedInputFilePatternAction(InputFilePattern InputFilePattern);
    public record SetSearchQueryAction(string SearchQuery);
    public record MoveBackwardsInHistoryAction;
    public record MoveForwardsInHistoryAction;
    public record OpenParentDirectoryAction(IBlazorStudioComponentRenderers BlazorStudioComponentRenderers, IFileSystemProvider FileSystemProvider, IEnvironmentProvider EnvironmentProvider);
    public record RefreshCurrentSelectionAction;
    public record StartInputFileStateFormAction(RequestInputFileStateFormAction RequestInputFileStateFormAction);
}