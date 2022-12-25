using System.Collections.Immutable;
using BlazorALaCarte.DialogNotification.NotificationCase;
using BlazorALaCarte.Shared.Clipboard;
using BlazorALaCarte.Shared.Menu;
using BlazorStudio.ClassLib.Clipboard;
using BlazorStudio.ClassLib.CommandLine;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileTemplates;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using Fluxor;

namespace BlazorStudio.ClassLib.Menu;

public class CommonMenuOptionsFactory : ICommonMenuOptionsFactory
{
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IClipboardProvider _clipboardProvider;

    /// <summary>
    /// I could not get a dependency injected <see cref="IDispatcher"/>
    /// to work and instead added <see cref="IDispatcher"/> as an argument
    /// to methods in this file that need an <see cref="IDispatcher"/>
    /// </summary>
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
                    nameof(IFileFormRendererType.CheckForTemplates),
                    false
                },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitAction),
                    new Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>>(
                        (fileName, exactMatchFileTemplate, relatedMatchFileTemplates) => 
                            PerformNewFileAction(
                                fileName, 
                                exactMatchFileTemplate, 
                                relatedMatchFileTemplates, 
                                new NamespacePath(
                                    string.Empty, 
                                    parentDirectory), 
                                onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord NewTemplatedFile(
        NamespacePath parentDirectory,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "New Templated File",
            MenuOptionKind.Create,
            WidgetRendererType: _commonComponentRenderers.FileFormRendererType,
            WidgetParameters: new Dictionary<string, object?>
            {
                {
                    nameof(IFileFormRendererType.FileName),
                    string.Empty
                },
                {
                    nameof(IFileFormRendererType.CheckForTemplates),
                    true
                },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitAction),
                    new Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>>(
                        (fileName, exactMatchFileTemplate, relatedMatchFileTemplates) => 
                            PerformNewFileAction(
                                fileName,
                                exactMatchFileTemplate,
                                relatedMatchFileTemplates,
                                parentDirectory,
                                onAfterCompletion))
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
                    new Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>>(
                        (directoryName, exactMatchFileTemplate, relatedMatchFileTemplates) => 
                            PerformNewDirectoryAction(
                                directoryName,
                                parentDirectory,
                                onAfterCompletion))
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
        IAbsoluteFilePath sourceAbsoluteFilePath,
        IDispatcher dispatcher,
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
                    sourceAbsoluteFilePath.IsDirectory
                        ? sourceAbsoluteFilePath.FileNameNoExtension
                        : sourceAbsoluteFilePath.FilenameWithExtension
                },
                {
                    nameof(IFileFormRendererType.IsDirectory),
                    sourceAbsoluteFilePath.IsDirectory
                },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitAction),
                    new Action<string, IFileTemplate?, ImmutableArray<IFileTemplate>>(
                        (nextName, exactMatchFileTemplate, relatedMatchFileTemplates) => 
                            PerformRenameAction(
                                sourceAbsoluteFilePath,
                                nextName,
                                dispatcher,
                                onAfterCompletion))
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

    public MenuOptionRecord RemoveCSharpProjectReferenceFromSolution(
        TreeViewNamespacePath? solutionNode,
        TreeViewNamespacePath projectNode,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Remove (no files are deleted)",
            MenuOptionKind.Delete,
            WidgetRendererType: _commonComponentRenderers.RemoveCSharpProjectFromSolutionRendererType,
            WidgetParameters: new Dictionary<string, object?>
            {
                {
                    nameof(IRemoveCSharpProjectFromSolutionRendererType.AbsoluteFilePath),
                    projectNode.Item?.AbsoluteFilePath
                },
                {
                    nameof(IDeleteFileFormRendererType.OnAfterSubmitAction),
                    new Action<IAbsoluteFilePath>(afp => 
                        PerformRemoveCSharpProjectReferenceFromSolutionAction(
                            solutionNode, 
                            projectNode,
                            terminalSession,
                            dispatcher,
                            onAfterCompletion))
                },
            });
    }

    public MenuOptionRecord AddProjectToProjectReference(
        TreeViewNamespacePath projectReceivingReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        return new MenuOptionRecord(
            "Add Project Reference",
            MenuOptionKind.Other,
            OnClick: () => PerformProjectToProjectReferenceAction(
                projectReceivingReference,
                terminalSession,
                dispatcher,
                onAfterCompletion));
    }

    private void PerformNewFileAction(
        string fileName,
        IFileTemplate? exactMatchFileTemplate,
        ImmutableArray<IFileTemplate> relatedMatchFileTemplates,
        NamespacePath namespacePath, 
        Func<Task> onAfterCompletion)
    {
        _ = Task.Run(async () =>
        {
            if (exactMatchFileTemplate is null)
            {
                var emptyFileAbsoluteFilePathString = namespacePath.AbsoluteFilePath
                                                          .GetAbsoluteFilePathString() +
                                                      fileName;

                var emptyFileAbsoluteFilePath = new AbsoluteFilePath(
                    emptyFileAbsoluteFilePathString, 
                    false);
                
                await _fileSystemProvider.WriteFileAsync(
                    emptyFileAbsoluteFilePath,
                    string.Empty,
                    false,
                    true); 
            }
            else
            {
                var allTemplates = new[] { exactMatchFileTemplate }
                    .Union(relatedMatchFileTemplates)
                    .ToArray();
                
                foreach (var fileTemplate in allTemplates)
                {
                    var templateResult = fileTemplate.ConstructFileContents.Invoke(
                        new FileTemplateParameter(
                            fileName,
                            namespacePath));
                    
                    await _fileSystemProvider.WriteFileAsync(
                        templateResult.FileNamespacePath.AbsoluteFilePath,
                        templateResult.Contents,
                        false,
                        true); 
                }
            }

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
        IDispatcher dispatcher,
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
                dispatcher,
                () => Task.CompletedTask);

            if (temporaryRenameResult is null)
            {
                onAfterCompletion.Invoke();
                return null;
            }
            else
                sourceAbsoluteFilePath = temporaryRenameResult;
        }
        
        var sourceAbsoluteFilePathString = sourceAbsoluteFilePath.GetAbsoluteFilePathString();
        
        var parentOfSource = (IAbsoluteFilePath)sourceAbsoluteFilePath.Directories.Last();

        var destinationAbsoluteFilePathString = parentOfSource.GetAbsoluteFilePathString() +
                                  nextName;

        sourceAbsoluteFilePathString = RemoveEndingDirectorySeparator(
            sourceAbsoluteFilePathString);
        
        destinationAbsoluteFilePathString = RemoveEndingDirectorySeparator(
            destinationAbsoluteFilePathString);
        
        try
        {
            if (sourceAbsoluteFilePath.IsDirectory)
            {
                System.IO.Directory.Move(
                    sourceAbsoluteFilePathString, 
                    destinationAbsoluteFilePathString);    
            }
            else
            {
                System.IO.File.Move(
                    sourceAbsoluteFilePathString, 
                    destinationAbsoluteFilePathString);
            }
        }
        catch (Exception e)
        {
            var notificationError  = new NotificationRecord(
                NotificationKey.NewNotificationKey(), 
                "Rename Action",
                _commonComponentRenderers.ErrorNotificationRendererType,
                new Dictionary<string, object?>
                {
                    {
                        nameof(IErrorNotificationRendererType.Message), 
                        $"ERROR: {e.Message}"
                    },
                },
                TimeSpan.FromSeconds(15));
        
            dispatcher.Dispatch(
                new NotificationsState.RegisterNotificationRecordAction(
                    notificationError));
            
            onAfterCompletion.Invoke();
            return null;
        }

        onAfterCompletion.Invoke();
        
        return new AbsoluteFilePath(
            destinationAbsoluteFilePathString,
            sourceAbsoluteFilePath.IsDirectory);
    }
    
    private void PerformRemoveCSharpProjectReferenceFromSolutionAction(TreeViewNamespacePath? solutionNode,
        TreeViewNamespacePath? projectNode,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        _ = Task.Run(async () =>
        { 
            if (solutionNode?.Item is not null &&
                projectNode?.Item is not null)
            {
                var workingDirectory = (IAbsoluteFilePath)solutionNode
                    .Item.AbsoluteFilePath.Directories.Last();

                var removeCSharpProjectReferenceFromSolutionCommandString = 
                    DotNetCliFacts.FormatRemoveCSharpProjectReferenceFromSolutionAction(
                        solutionNode.Item.AbsoluteFilePath
                            .GetAbsoluteFilePathString(),
                        projectNode.Item.AbsoluteFilePath
                            .GetAbsoluteFilePathString());
            
                var removeCSharpProjectReferenceFromSolutionCommand = new TerminalCommand(
                    TerminalCommandKey.NewTerminalCommandKey(), 
                    removeCSharpProjectReferenceFromSolutionCommandString,
                    workingDirectory.GetAbsoluteFilePathString(),
                    CancellationToken.None,
                    async () =>
                    {
                        // Re-open the modified Solution
                        dispatcher.Dispatch(
                            new SolutionExplorerState.RequestSetSolutionExplorerStateAction(
                                solutionNode.Item.AbsoluteFilePath));
                    });
        
                await terminalSession
                    .EnqueueCommandAsync(
                        removeCSharpProjectReferenceFromSolutionCommand);
            }

            await onAfterCompletion.Invoke();
        });
    }
    
    public void PerformProjectToProjectReferenceAction(TreeViewNamespacePath projectReceivingReference,
        TerminalSession terminalSession,
        IDispatcher dispatcher,
        Func<Task> onAfterCompletion)
    {
        _ = Task.Run(async () =>
        {
            if (projectReceivingReference.Item is null)
            {
                await onAfterCompletion.Invoke();
                return;
            }

            var requestInputFileStateFormAction = new InputFileState.RequestInputFileStateFormAction(
                $"Add Project reference to {projectReceivingReference.Item.AbsoluteFilePath.FilenameWithExtension}",
                async referencedProject =>
                {
                    if (referencedProject is null)
                        return;

                    var interpolatedCommand = DotNetCliFacts
                        .FormatAddProjectToProjectReference(
                            projectReceivingReference.Item.AbsoluteFilePath.GetAbsoluteFilePathString(),
                            referencedProject.GetAbsoluteFilePathString());

                    var addExistingProjectToSolutionTerminalCommand = new TerminalCommand(
                        TerminalCommandKey.NewTerminalCommandKey(),
                        interpolatedCommand,
                        null,
                        CancellationToken.None,
                        async () =>
                        {
                            var notificationInformative = new NotificationRecord(
                                NotificationKey.NewNotificationKey(),
                                "Add Project Reference",
                                _commonComponentRenderers.InformativeNotificationRendererType,
                                new Dictionary<string, object?>
                                {
                                    {
                                        nameof(IInformativeNotificationRendererType.Message),
                                        $"Modified {projectReceivingReference.Item.AbsoluteFilePath.FilenameWithExtension}" +
                                        $" to have a reference to {referencedProject.FilenameWithExtension}"
                                    },
                                },
                                TimeSpan.FromSeconds(7));

                            dispatcher.Dispatch(
                                new NotificationsState.RegisterNotificationRecordAction(
                                    notificationInformative));

                            await onAfterCompletion.Invoke();
                        });

                    await terminalSession
                        .EnqueueCommandAsync(addExistingProjectToSolutionTerminalCommand);
                },
                afp =>
                {
                    if (afp is null ||
                        afp.IsDirectory)
                    {
                        return Task.FromResult(false);
                    }

                    return Task.FromResult(
                        afp.ExtensionNoPeriod
                            .EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT));
                },
                new[]
                {
                    new InputFilePattern(
                        "C# Project",
                        afp =>
                            afp.ExtensionNoPeriod
                                .EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
                }.ToImmutableArray());
        
            dispatcher.Dispatch(
                requestInputFileStateFormAction);
        });
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
}