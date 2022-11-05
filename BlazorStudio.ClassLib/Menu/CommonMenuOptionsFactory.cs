using BlazorStudio.ClassLib.Clipboard;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorTextEditor.RazorLib.Clipboard;

namespace BlazorStudio.ClassLib.Menu;

public class CommonMenuOptionsFactory : ICommonMenuOptionsFactory
{
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IClipboardProvider _clipboardProvider;

    public CommonMenuOptionsFactory(
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IClipboardProvider clipboardProvider)
    {
        _commonComponentRenderers = commonComponentRenderers;
        _fileSystemProvider = fileSystemProvider;
        _clipboardProvider = clipboardProvider;
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

    public MenuOptionRecord DeleteFile(
        IAbsoluteFilePath absoluteFilePath, 
        Func<Task> onAfterCompletion)
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
    
    public MenuOptionRecord RenameFile(
        IAbsoluteFilePath absoluteFilePath,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Rename",
            MenuOptionKind.Update,
            WidgetRendererType: _commonComponentRenderers.FileFormRendererType,
            WidgetParameters: new Dictionary<string, object?>
            {
                {
                    nameof(IFileFormRendererType.FileName),
                    absoluteFilePath.IsDirectory
                        ? absoluteFilePath.FileNameNoExtension
                        : absoluteFilePath.FilenameWithExtension
                },
                {
                    nameof(IFileFormRendererType.IsDirectory),
                    absoluteFilePath.IsDirectory
                },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitAction),
                    new Action<string>(nextName => 
                        PerformRenameAction(absoluteFilePath, nextName, onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord CopyFile(
        IAbsoluteFilePath absoluteFilePath,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Copy",
            MenuOptionKind.Update,
            OnClick: () => PerformCopyFileAction(
                absoluteFilePath, 
                onAfterCompletion));
    }
    
    public MenuOptionRecord CutFile(
        IAbsoluteFilePath absoluteFilePath,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Cut",
            MenuOptionKind.Update,
            OnClick: () => PerformCutFileAction(
                absoluteFilePath, 
                onAfterCompletion));
    }
    
    public MenuOptionRecord PasteClipboard(
        IAbsoluteFilePath directoryAbsoluteFilePath,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Paste",
            MenuOptionKind.Update,
            OnClick: () => PerformPasteFileAction(
                directoryAbsoluteFilePath, 
                onAfterCompletion));
    }

    private void PerformNewEmptyFileAction(
        string fileName,
        IAbsoluteFilePath parentDirectory, 
        Func<Task> onAfterCompletion)
    {
        var emptyFileAbsoluteFilePathString = parentDirectory.GetAbsoluteFilePathString() +
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
    
    private void PerformNewDirectoryAction(
        string directoryName,
        IAbsoluteFilePath parentDirectory, 
        Func<Task> onAfterCompletion)
    {
        var directoryAbsoluteFilePathString = parentDirectory.GetAbsoluteFilePathString() +
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
    
    private void PerformDeleteFileAction(
        IAbsoluteFilePath absoluteFilePath, 
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
    
    private void PerformCopyFileAction(
        IAbsoluteFilePath absoluteFilePath, 
        Func<Task> onAfterCompletion)
    {
        _ = Task.Run(async () =>
        { 
            await _clipboardProvider
                .SetClipboard(
                    ClipboardFacts.FormatPhrase(
                        ClipboardFacts.CopyCommand,
                        ClipboardFacts.AbsoluteFilePathDataType,
                        absoluteFilePath.GetAbsoluteFilePathString()));

            await onAfterCompletion.Invoke();
        });
    }
    
    private void PerformCutFileAction(
        IAbsoluteFilePath absoluteFilePath, 
        Func<Task> onAfterCompletion)
    {
        _ = Task.Run(async () =>
        { 
            await _clipboardProvider
                .SetClipboard(
                    ClipboardFacts.FormatPhrase(
                        ClipboardFacts.CutCommand,
                        ClipboardFacts.AbsoluteFilePathDataType,
                        absoluteFilePath.GetAbsoluteFilePathString()));

            await onAfterCompletion.Invoke();
        });
    }
    
    private void PerformPasteFileAction(
        IAbsoluteFilePath receivingDirectory, 
        Func<Task> onAfterCompletion)
    {
        _ = Task.Run(async () =>
        {
            var clipboardContents = await _clipboardProvider
                .ReadClipboard();

            if (ClipboardFacts.TryParseString(
                    clipboardContents, out var clipboardPhrase))
            {
                if (clipboardPhrase is not null &&
                    clipboardPhrase.DataType == ClipboardFacts.AbsoluteFilePathDataType)
                {
                    if (clipboardPhrase.Command == ClipboardFacts.CopyCommand ||
                        clipboardPhrase.Command == ClipboardFacts.CutCommand)
                    {

                        IAbsoluteFilePath? clipboardAbsoluteFilePath = null;
                        
                        if (Directory.Exists(clipboardPhrase.Value))
                        {
                            clipboardPhrase.Value = RemoveEndingDirectorySeparator(
                                clipboardPhrase.Value);
                            
                            clipboardAbsoluteFilePath = new AbsoluteFilePath(
                                clipboardPhrase.Value,
                                true);
                        }
                        else if (File.Exists(clipboardPhrase.Value))
                        {
                            clipboardAbsoluteFilePath = new AbsoluteFilePath(
                                clipboardPhrase.Value,
                                false);
                        }

                        if (clipboardAbsoluteFilePath is not null)
                        {
                            var successfullyPasted = true;
                            
                            try
                            {
                                if (clipboardAbsoluteFilePath.IsDirectory)
                                {
                                    var clipboardDirectoryInfo =
                                        new DirectoryInfo(
                                            clipboardAbsoluteFilePath
                                                .GetAbsoluteFilePathString());
                                    
                                    var receivingDirectoryInfo =
                                        new DirectoryInfo(
                                            receivingDirectory
                                                .GetAbsoluteFilePathString());
                                    
                                    CopyFilesRecursively(
                                        clipboardDirectoryInfo,
                                        receivingDirectoryInfo);
                                }
                                else
                                {
                                    var destinationFileName = receivingDirectory.GetAbsoluteFilePathString() +
                                                              clipboardAbsoluteFilePath.FilenameWithExtension;
                                
                                    var sourceAbsoluteFilePathString = clipboardAbsoluteFilePath
                                        .GetAbsoluteFilePathString();
                                
                                    File.Copy(
                                        sourceAbsoluteFilePathString,
                                        destinationFileName);
                                }
                            }
                            catch (Exception e)
                            {
                                successfullyPasted = false; 
                            }

                            if (successfullyPasted &&
                                clipboardPhrase.Command == ClipboardFacts.CutCommand)
                            {
                                // TODO: Rerender the parent of the deleted due to cut file
                                PerformDeleteFileAction(
                                    clipboardAbsoluteFilePath,
                                    onAfterCompletion);    
                            }
                            else
                            {
                                await onAfterCompletion.Invoke();
                            }
                        }
                    }
                }
            }
        });
    }
    
    private IAbsoluteFilePath? PerformRenameAction(
        IAbsoluteFilePath sourceAbsoluteFilePath, 
        string nextName, 
        Func<Task> onAfterCompletion)
    {
        // If the current and next name match when compared
        // with case insensitivity
        if (string.Compare(
                sourceAbsoluteFilePath.FilenameWithExtension, 
                nextName, 
                StringComparison.OrdinalIgnoreCase)
                    == 0)
        {
            var temporaryNextName = Path.GetRandomFileName();
            
            var temporaryRenameResult = PerformRenameAction(
                sourceAbsoluteFilePath, 
                temporaryNextName, 
                () => Task.CompletedTask);

            if (temporaryRenameResult is null)
                return null;
            else
                sourceAbsoluteFilePath = temporaryRenameResult;
        }
        
        var sourceAbsoluteFilePathString = sourceAbsoluteFilePath.GetAbsoluteFilePathString();
        
        var parentOfSource = (IAbsoluteFilePath)sourceAbsoluteFilePath.Directories.Last();

        var destinationAbsoluteFilePathString = parentOfSource.GetAbsoluteFilePathString() +
                                  nextName;

        try
        {
            System.IO.File.Move(
                sourceAbsoluteFilePathString, 
                destinationAbsoluteFilePathString);
        }
        catch (Exception e)
        {
            // TODO: Dispatch a notification to the user of the error.
            return null;
        }

        return new AbsoluteFilePath(
            destinationAbsoluteFilePathString,
            sourceAbsoluteFilePath.IsDirectory);
    }

    private string RemoveEndingDirectorySeparator(string value)
    {
        if (value.EndsWith(Path.DirectorySeparatorChar) ||
            value.EndsWith(Path.AltDirectorySeparatorChar))
        {
            return value.Substring(
                0, 
                value.Length - 1);
        }

        return value;
    }
    
    /// <summary>
    /// Looking into copying and pasting a directory
    /// https://stackoverflow.com/questions/58744/copy-the-entire-contents-of-a-directory-in-c-sharp
    /// </summary>
    public static DirectoryInfo CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
    {
        var newDirectoryInfo = target.CreateSubdirectory(source.Name);
        foreach (var fileInfo in source.GetFiles())
            fileInfo.CopyTo(Path.Combine(newDirectoryInfo.FullName, fileInfo.Name));

        foreach (var childDirectoryInfo in source.GetDirectories())
            CopyFilesRecursively(childDirectoryInfo, newDirectoryInfo);

        return newDirectoryInfo;
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