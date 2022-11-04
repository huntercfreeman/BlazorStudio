using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.Menu;

public class CommonMenuOptionsFactory : ICommonMenuOptionsFactory
{
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;

    public CommonMenuOptionsFactory(
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider)
    {
        _commonComponentRenderers = commonComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
    }
    
    public MenuOptionRecord NewEmptyFile(
        IAbsoluteFilePath parentDirectory,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "New Empty File",
            MenuOptionKind.Create,
            WidgetRendererType: _commonComponentRenderers.FileFormRendererType,
            WidgetParameters: new Dictionary<string, object?>
            {
                {
                    nameof(IFileFormRendererType.FileName),
                    string.Empty
                },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitAction),
                    new Action<string>(fileName => 
                        PerformNewEmptyFileAction(fileName, parentDirectory, onAfterCompletion))
                },
            });
    }

    private void PerformNewEmptyFileAction(string fileName,
        IAbsoluteFilePath parentDirectory, 
        Func<Task> onAfterCompletion)
    {
        var emptyFileAbsoluteFilePathString = parentDirectory.GetAbsoluteFilePathString() +
                                              Path.DirectorySeparatorChar +
                                              fileName;

        var emptyFileAbsoluteFilePath = new AbsoluteFilePath(
            emptyFileAbsoluteFilePathString, 
            false);

        _ = Task.Run(async () =>
        {
            await _fileSystemProvider.WriteFileAsync(
                emptyFileAbsoluteFilePath,
                string.Empty,
                false,
                true);

            await onAfterCompletion.Invoke();
        });
    }
    /*
 * -New
        -Empty File
        -Templated File
            -Add a Codebehind Prompt when applicable
    -Base file operations
        -Copy
        -Delete
        -Cut
        -Rename
    -Base directory operations
        -Copy
        -Delete
        -Cut
        -Rename
        -Paste
 */
}