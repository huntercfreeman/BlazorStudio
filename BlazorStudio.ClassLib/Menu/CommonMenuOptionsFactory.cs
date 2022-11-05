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
    
    public MenuOptionRecord NewDirectory(
        IAbsoluteFilePath parentDirectory,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "New Directory",
            MenuOptionKind.Create,
            WidgetRendererType: _commonComponentRenderers.FileFormRendererType,
            WidgetParameters: new Dictionary<string, object?>
            {
                {
                    nameof(IFileFormRendererType.FileName),
                    string.Empty
                },
                {
                    nameof(IFileFormRendererType.IsDirectory),
                    true
                },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitAction),
                    new Action<string>(directoryName => 
                        PerformNewDirectoryAction(directoryName, parentDirectory, onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord DeleteFile(IAbsoluteFilePath absoluteFilePath, Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Delete",
            MenuOptionKind.Delete,
            WidgetRendererType: _commonComponentRenderers.DeleteFileFormRendererType,
            WidgetParameters: new Dictionary<string, object?>
            {
                {
                    nameof(IDeleteFileFormRendererType.AbsoluteFilePath),
                    absoluteFilePath
                },
                {
                    nameof(IDeleteFileFormRendererType.IsDirectory),
                    true
                },
                {
                    nameof(IDeleteFileFormRendererType.OnAfterSubmitAction),
                    new Action<IAbsoluteFilePath>(afp => 
                        PerformDeleteFileAction(afp, onAfterCompletion))
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
    
    private void PerformNewDirectoryAction(string directoryName,
        IAbsoluteFilePath parentDirectory, 
        Func<Task> onAfterCompletion)
    {
        var directoryAbsoluteFilePathString = parentDirectory.GetAbsoluteFilePathString() +
                                              Path.DirectorySeparatorChar +
                                              directoryName;

        var directoryAbsoluteFilePath = new AbsoluteFilePath(
            directoryAbsoluteFilePathString, 
            false);

        _ = Task.Run(async () =>
        {
            await _fileSystemProvider.CreateDirectoryAsync(
                directoryAbsoluteFilePath);

            await onAfterCompletion.Invoke();
        });
    }
    
    private void PerformDeleteFileAction(IAbsoluteFilePath absoluteFilePath, 
        Func<Task> onAfterCompletion)
    {
        _ = Task.Run(async () =>
        {
            if (absoluteFilePath.IsDirectory)
            {
                await _fileSystemProvider.DeleteDirectoryAsync(
                    absoluteFilePath,
                    true);    
            }
            else
            {
                await _fileSystemProvider.DeleteFileAsync(
                    absoluteFilePath);
            }

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