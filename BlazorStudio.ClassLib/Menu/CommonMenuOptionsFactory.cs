﻿using BlazorStudio.ClassLib.Clipboard;
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
    
    private void PerformNewDirectoryAction(
        string directoryName,
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
                        var successfullyPasted = false;

                        IAbsoluteFilePath? clipboardAbsoluteFilePath = null;
                        
                        if (Directory.Exists(clipboardPhrase.Value))
                        {
                            clipboardAbsoluteFilePath = new AbsoluteFilePath(
                                clipboardPhrase.Value,
                                true);

                            var pasted = await PerformPasteDirectoryContentAction(
                                receivingDirectory,
                                clipboardAbsoluteFilePath);

                            if (pasted)
                                successfullyPasted = true;
                        }
                        else if (File.Exists(clipboardPhrase.Value))
                        {
                            clipboardAbsoluteFilePath = new AbsoluteFilePath(
                                clipboardPhrase.Value,
                                false);
                        
                            var pasted = await PerformPasteFileContentAction(
                                receivingDirectory,
                                clipboardAbsoluteFilePath);
                        
                            if (pasted)
                                successfullyPasted = true;
                        }

                        if (successfullyPasted &&
                            clipboardAbsoluteFilePath is not null)
                        {
                            if (clipboardPhrase.Command == ClipboardFacts.CutCommand)
                            {
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
    
    private async Task<bool> PerformPasteDirectoryContentAction(
        IAbsoluteFilePath receivingDirectory,
        IAbsoluteFilePath directoryFromClipboard)
    {
    }
    
    private async Task<bool> PerformPasteFileContentAction(
        IAbsoluteFilePath receivingDirectory,
        IAbsoluteFilePath fileFromClipboard)
    {
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