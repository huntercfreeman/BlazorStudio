using System.Collections.Immutable;
using System.Diagnostics;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.RoslynHelpers;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.FooterWindowCase;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.ClassLib.Store.NugetPackageManagerCase;
using BlazorStudio.ClassLib.Store.RoslynWorkspaceState;
using BlazorStudio.ClassLib.Store.SolutionCase;
using BlazorStudio.ClassLib.Store.SolutionExplorerCase;
using BlazorStudio.ClassLib.Store.StartupProject;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.ClassLib.Templates;
using BlazorStudio.RazorLib.Forms;
using BlazorStudio.RazorLib.InputFile;
using BlazorStudio.RazorLib.TreeViewCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.CSharp;

namespace BlazorStudio.RazorLib.SolutionExplorer;

public partial class SolutionExplorerMenuWrapperDisplay : ComponentBase
{
    private DialogKey _addProjectReferenceDialogKey = DialogKey.NewDialogKey();
    private DialogKey _syntaxRootDisplayDialogKey = DialogKey.NewDialogKey();
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IState<SolutionExplorerState> SolutionExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IState<SolutionState> SolutionStateWrap { get; set; } = null!;
    [Inject]
    private IState<RoslynWorkspaceState> RoslynWorkspaceStateWrap { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    public TreeViewContextMenuEventDto<AbsoluteFilePathDotNet> ContextMenuEventDto { get; set; } = null!;

    private IEnumerable<MenuOptionRecord> GetMenuOptionRecords(
        TreeViewContextMenuEventDto<AbsoluteFilePathDotNet> contextMenuEventDto)
    {
        var createNewEmptyFile = MenuOptionFacts.File
            .ConstructCreateNewEmptyFile(typeof(CreateNewFileForm),
                new Dictionary<string, object?>
                {
                    {
                        nameof(CreateNewFileForm.ParentDirectory),
                        contextMenuEventDto.Item
                    },
                    {
                        nameof(CreateNewFileForm.OnAfterSubmitForm),
                        new Action<string, string, bool>((parentDirectoryAbsoluteFilePathString, filename, _) =>
                            CreateNewEmptyFileFormOnAfterSubmitForm(parentDirectoryAbsoluteFilePathString,
                                filename,
                                ContextMenuEventDto.Item))
                    },
                    {
                        nameof(CreateNewFileForm.OnAfterCancelForm),
                        new Action(() => Dispatcher.Dispatch(new ClearActiveDropdownKeysAction()))
                    },
                });

        var createNewTemplatedFile = MenuOptionFacts.File
            .ConstructCreateNewTemplatedFile(typeof(CreateNewFileForm),
                new Dictionary<string, object?>
                {
                    {
                        nameof(CreateNewFileForm.ParentDirectory),
                        contextMenuEventDto.Item
                    },
                    {
                        nameof(CreateNewFileForm.OnAfterSubmitForm),
                        new Action<string, string, bool>(
                            (parentDirectoryAbsoluteFilePathString, filename, shouldAddCodebehind) =>
                                CreateNewTemplatedFileFormOnAfterSubmitForm(parentDirectoryAbsoluteFilePathString,
                                    filename,
                                    ContextMenuEventDto.Item,
                                    shouldAddCodebehind))
                    },
                    {
                        nameof(CreateNewFileForm.IsTemplated),
                        true
                    },
                    {
                        nameof(CreateNewFileForm.OnAfterCancelForm),
                        new Action(() => Dispatcher.Dispatch(new ClearActiveDropdownKeysAction()))
                    },
                });

        var createNewDirectory = MenuOptionFacts.File
            .ConstructCreateNewDirectory(typeof(CreateNewDirectoryForm),
                new Dictionary<string, object?>
                {
                    {
                        nameof(CreateNewDirectoryForm.ParentDirectory),
                        contextMenuEventDto.Item
                    },
                    {
                        nameof(CreateNewDirectoryForm.OnAfterSubmitForm),
                        new Action<string, string>(CreateNewDirectoryFormOnAfterSubmitForm)
                    },
                    {
                        nameof(CreateNewDirectoryForm.OnAfterCancelForm),
                        new Action(() => Dispatcher.Dispatch(new ClearActiveDropdownKeysAction()))
                    },
                });

        List<MenuOptionRecord> menuOptionRecords = new();

        if (contextMenuEventDto.Item.IsDirectory)
        {
            menuOptionRecords.Add(createNewTemplatedFile);
            menuOptionRecords.Add(createNewEmptyFile);
            menuOptionRecords.Add(createNewDirectory);
        }

        if (contextMenuEventDto.Item.ExtensionNoPeriod == ExtensionNoPeriodFacts.C_SHARP_PROJECT)
        {
            var setAsStartupProject = MenuOptionFacts.CSharp
                .SetAsStartupProject(() =>
                    Dispatcher.Dispatch(new SetStartupProjectAction(contextMenuEventDto.Item)));

            DialogRecord addProjectReferenceDialog = null;

            void AddProjectReferenceConfirmOnClickOverrideAction(ImmutableArray<IAbsoluteFilePath> activeItems)
            {
                Dispatcher.Dispatch(new DisposeDialogAction(addProjectReferenceDialog));

                var localSolutionExplorerState = SolutionExplorerStateWrap.Value;

                var referenceAbsoluteFilePathString = activeItems[0].GetAbsoluteFilePathString();

                var contextMenuTargetAbsoluteFilePathString =
                    contextMenuEventDto.Item.GetAbsoluteFilePathString();

                void OnStart()
                {
                }

                void OnEnd(Process finishedProcess)
                {
                    var localMsBuildWorkspace = RoslynWorkspaceStateWrap.Value.MsBuildWorkspace;

                    if (localMsBuildWorkspace is not null)
                    {
                        localMsBuildWorkspace.CloseSolution();

                        Dispatcher.Dispatch(new SetSolutionExplorerAction(
                            localSolutionExplorerState.SolutionAbsoluteFilePath, SequenceKey.NewSequenceKey()));
                    }
                }

                var command = $"dotnet add {contextMenuTargetAbsoluteFilePathString}" +
                              $" reference {referenceAbsoluteFilePathString}";

                Dispatcher
                    .Dispatch(new EnqueueProcessOnTerminalEntryAction(
                        TerminalStateFacts.GeneralTerminalEntry.TerminalEntryKey,
                        command,
                        null,
                        OnStart,
                        OnEnd,
                        null,
                        null,
                        null,
                        CancellationToken.None));
            }

            addProjectReferenceDialog = new DialogRecord(
                DialogKey.NewDialogKey(),
                "Add Project Reference",
                typeof(InputFileDialog),
                new Dictionary<string, object?>
                {
                    {
                        nameof(InputFileDialog.IsValidSelectionOverrideFunc),
                        new Func<ImmutableArray<IAbsoluteFilePath>, bool>(
                            AddProjectReferenceInputIsValidOverride)
                    },
                    {
                        nameof(InputFileDialog.InvalidSelectionTextOverride),
                        "Choose a C# Project"
                    },
                    {
                        nameof(InputFileDialog.ConfirmOnClickOverrideAction),
                        new Action<ImmutableArray<IAbsoluteFilePath>>(
                            AddProjectReferenceConfirmOnClickOverrideAction)
                    },
                }
            );

            var addProjectReference = MenuOptionFacts.CSharp
                .AddProjectReference(() =>
                {
                    if (DialogStatesWrap.Value.List.All(x => x.DialogKey != _addProjectReferenceDialogKey))
                        Dispatcher.Dispatch(new RegisterDialogAction(addProjectReferenceDialog));
                });

            var containingDirectory = contextMenuEventDto.Item.Directories.Last();

            createNewEmptyFile = MenuOptionFacts.File
                .ConstructCreateNewEmptyFile(typeof(CreateNewFileForm),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CreateNewFileForm.ParentDirectory),
                            containingDirectory
                        },
                        {
                            nameof(CreateNewFileForm.OnAfterSubmitForm),
                            new Action<string, string, bool>((parentDirectoryAbsoluteFilePathString, filename, _) =>
                                CreateNewEmptyFileFormOnAfterSubmitForm(parentDirectoryAbsoluteFilePathString,
                                    filename,
                                    ContextMenuEventDto.Item))
                        },
                        {
                            nameof(CreateNewFileForm.OnAfterCancelForm),
                            new Action(() => Dispatcher.Dispatch(new ClearActiveDropdownKeysAction()))
                        },
                    });

            createNewTemplatedFile = MenuOptionFacts.File
                .ConstructCreateNewTemplatedFile(typeof(CreateNewFileForm),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CreateNewFileForm.ParentDirectory),
                            containingDirectory
                        },
                        {
                            nameof(CreateNewFileForm.OnAfterSubmitForm),
                            new Action<string, string, bool>(
                                (parentDirectoryAbsoluteFilePathString, filename, shouldAddCodebehind) =>
                                    CreateNewTemplatedFileFormOnAfterSubmitForm(parentDirectoryAbsoluteFilePathString,
                                        filename,
                                        ContextMenuEventDto.Item,
                                        shouldAddCodebehind))
                        },
                        {
                            nameof(CreateNewFileForm.IsTemplated),
                            true
                        },
                        {
                            nameof(CreateNewFileForm.OnAfterCancelForm),
                            new Action(() => Dispatcher.Dispatch(new ClearActiveDropdownKeysAction()))
                        },
                    });

            createNewDirectory = MenuOptionFacts.File
                .ConstructCreateNewDirectory(typeof(CreateNewDirectoryForm),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CreateNewDirectoryForm.ParentDirectory),
                            containingDirectory
                        },
                        {
                            nameof(CreateNewDirectoryForm.OnAfterSubmitForm),
                            new Action<string, string>(CreateNewDirectoryFormOnAfterSubmitForm)
                        },
                        {
                            nameof(CreateNewDirectoryForm.OnAfterCancelForm),
                            new Action(() => Dispatcher.Dispatch(new ClearActiveDropdownKeysAction()))
                        },
                    });

            var requestFocusOnNugetPackageManager = new MenuOptionRecord(
                MenuOptionKey.NewMenuOptionKey(),
                "Nuget Packages",
                ImmutableList<MenuOptionRecord>.Empty,
                () =>
                {
                    Dispatcher.Dispatch(new SetActiveFooterWindowKindAction(FooterWindowKind.NugetPackageManager));
                    Dispatcher.Dispatch(new RequestFocusOnNugetPackageManagerAction());
                },
                MenuOptionKind.Update);

            menuOptionRecords.Add(createNewTemplatedFile);
            menuOptionRecords.Add(createNewEmptyFile);
            menuOptionRecords.Add(createNewDirectory);
            menuOptionRecords.Add(setAsStartupProject);
            menuOptionRecords.Add(addProjectReference);
            menuOptionRecords.Add(requestFocusOnNugetPackageManager);
        }

        if (contextMenuEventDto.Item.ExtensionNoPeriod != ExtensionNoPeriodFacts.C_SHARP_PROJECT)
        {
            var createDeleteFile = MenuOptionFacts.File
                .ConstructDeleteFile(typeof(DeleteFileForm),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(DeleteFileForm.AbsoluteFilePath),
                            contextMenuEventDto.Item
                        },
                        {
                            nameof(DeleteFileForm.OnAfterSubmitForm),
                            new Action<IAbsoluteFilePath>(x =>
                                DeleteFileFormOnAfterSubmitForm((AbsoluteFilePathDotNet)x))
                        },
                        {
                            nameof(DeleteFileForm.OnAfterCancelForm),
                            new Action(() => Dispatcher.Dispatch(new ClearActiveDropdownKeysAction()))
                        },
                    });

            menuOptionRecords.Add(createDeleteFile);
        }

        return menuOptionRecords.Any()
            ? menuOptionRecords
            : new[]
            {
                new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
                    "No Context Menu Options for this item",
                    ImmutableList<MenuOptionRecord>.Empty,
                    null,
                    MenuOptionKind.Read),
            };
    }

    private void CreateNewEmptyFileFormOnAfterSubmitForm(string parentDirectoryAbsoluteFilePathString,
        string fileName,
        AbsoluteFilePathDotNet absoluteFilePathDotNet)
    {
        var localRefreshContextMenuTarget = ContextMenuEventDto.RefreshContextMenuTarget;

        _ = TaskModelManagerService.EnqueueTaskModelAsync(async cancellationToken =>
            {
                var newFile = new AbsoluteFilePathDotNet(parentDirectoryAbsoluteFilePathString + fileName,
                    false,
                    absoluteFilePathDotNet.ProjectId);

                await File
                    .AppendAllTextAsync(parentDirectoryAbsoluteFilePathString + fileName,
                        string.Empty);

                await localRefreshContextMenuTarget();

                Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());

                var solutionState = SolutionStateWrap.Value;

                var project = SolutionStateWrap.Value.ProjectIdToProjectMap[absoluteFilePathDotNet.ProjectId];

                if (newFile.ExtensionNoPeriod == ExtensionNoPeriodFacts.C_SHARP_CLASS)
                {
                    var syntaxTree =
                        CSharpSyntaxTree.ParseText(await File.ReadAllTextAsync(newFile.GetAbsoluteFilePathString()));

                    var document = project.Project.AddDocument(fileName, await syntaxTree.GetRootAsync());

                    var nextProjectIdToProjectMap =
                        SolutionStateWrap.Value.ProjectIdToProjectMap
                            .SetItem(project.Project.Id,
                                new IndexedProject(document.Project, project.AbsoluteFilePathDotNet));

                    var nextDocumentMap =
                        SolutionStateWrap.Value.FileAbsoluteFilePathToDocumentMap
                            .SetItem(
                                new AbsoluteFilePathStringValue(newFile),
                                new IndexedDocument(document, newFile));

                    Dispatcher.Dispatch(new SetSolutionStateAction(solutionState.Solution,
                        nextProjectIdToProjectMap,
                        nextDocumentMap,
                        solutionState.FileAbsoluteFilePathToAdditionalDocumentMap));
                }
                else
                {
                    var document = project.Project
                        .AddDocument(
                            fileName,
                            await File.ReadAllTextAsync(newFile.GetAbsoluteFilePathString()));

                    var nextProjectIdToProjectMap =
                        SolutionStateWrap.Value.ProjectIdToProjectMap
                            .SetItem(project.Project.Id,
                                new IndexedProject(document.Project, project.AbsoluteFilePathDotNet));

                    var nextAdditionalDocumentMap =
                        SolutionStateWrap.Value.FileAbsoluteFilePathToAdditionalDocumentMap
                            .SetItem(
                                new AbsoluteFilePathStringValue(newFile),
                                new IndexedAdditionalDocument(document, newFile));

                    Dispatcher.Dispatch(new SetSolutionStateAction(solutionState.Solution,
                        nextProjectIdToProjectMap,
                        solutionState.FileAbsoluteFilePathToDocumentMap,
                        nextAdditionalDocumentMap));
                }
            },
            $"{nameof(CreateNewEmptyFileFormOnAfterSubmitForm)}",
            false,
            TimeSpan.FromSeconds(10));
    }

    private void CreateNewTemplatedFileFormOnAfterSubmitForm(string parentDirectoryAbsoluteFilePathString,
        string fileName,
        AbsoluteFilePathDotNet absoluteFilePathDotNet,
        bool shouldAddCodebehind)
    {
        var localRefreshContextMenuTarget = ContextMenuEventDto.RefreshContextMenuTarget;

        _ = TaskModelManagerService.EnqueueTaskModelAsync(async cancellationToken =>
            {
                var newFile = new AbsoluteFilePathDotNet(parentDirectoryAbsoluteFilePathString + fileName,
                    false,
                    absoluteFilePathDotNet.ProjectId);

                var solutionState = SolutionStateWrap.Value;

                var project = SolutionStateWrap.Value.ProjectIdToProjectMap[absoluteFilePathDotNet.ProjectId];

                var namespaceString = project.Project.DefaultNamespace;

                if (string.IsNullOrWhiteSpace(namespaceString)) namespaceString = project.Project.Name;

                await File
                    .AppendAllTextAsync(newFile.GetAbsoluteFilePathString(),
                        DotNetTemplates
                            .GetTemplate(
                                newFile.FilenameWithExtension.EndsWith(ExtensionNoPeriodFacts.RAZOR_CODEBEHIND)
                                    ? ExtensionNoPeriodFacts.RAZOR_CODEBEHIND
                                    : newFile.ExtensionNoPeriod,
                                namespaceString,
                                newFile.FileNameNoExtension));

                await localRefreshContextMenuTarget();

                Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());

                if (newFile.ExtensionNoPeriod == ExtensionNoPeriodFacts.C_SHARP_CLASS)
                {
                    var syntaxTree =
                        CSharpSyntaxTree.ParseText(await File.ReadAllTextAsync(newFile.GetAbsoluteFilePathString()));

                    var document = project.Project.AddDocument(fileName, await syntaxTree.GetRootAsync());

                    var nextProjectIdToProjectMap =
                        SolutionStateWrap.Value.ProjectIdToProjectMap
                            .SetItem(project.Project.Id,
                                new IndexedProject(document.Project, project.AbsoluteFilePathDotNet));

                    var nextDocumentMap =
                        SolutionStateWrap.Value.FileAbsoluteFilePathToDocumentMap
                            .SetItem(
                                new AbsoluteFilePathStringValue(newFile),
                                new IndexedDocument(document, newFile));

                    Dispatcher.Dispatch(new SetSolutionStateAction(solutionState.Solution,
                        nextProjectIdToProjectMap,
                        nextDocumentMap,
                        solutionState.FileAbsoluteFilePathToAdditionalDocumentMap));
                }
                else
                {
                    var document = project.Project
                        .AddDocument(
                            fileName,
                            await File.ReadAllTextAsync(newFile.GetAbsoluteFilePathString()));

                    var nextProjectIdToProjectMap =
                        SolutionStateWrap.Value.ProjectIdToProjectMap
                            .SetItem(project.Project.Id,
                                new IndexedProject(document.Project, project.AbsoluteFilePathDotNet));

                    var nextAdditionalDocumentMap =
                        SolutionStateWrap.Value.FileAbsoluteFilePathToAdditionalDocumentMap
                            .SetItem(
                                new AbsoluteFilePathStringValue(newFile),
                                new IndexedAdditionalDocument(document, newFile));

                    Dispatcher.Dispatch(new SetSolutionStateAction(solutionState.Solution,
                        nextProjectIdToProjectMap,
                        solutionState.FileAbsoluteFilePathToDocumentMap,
                        nextAdditionalDocumentMap));
                }

                if (shouldAddCodebehind && newFile.ExtensionNoPeriod == ExtensionNoPeriodFacts.RAZOR_MARKUP)
                {
                    CreateNewTemplatedFileFormOnAfterSubmitForm(parentDirectoryAbsoluteFilePathString,
                        fileName + '.' + ExtensionNoPeriodFacts.C_SHARP_CLASS,
                        absoluteFilePathDotNet,
                        false);
                }
            },
            $"{nameof(CreateNewTemplatedFileFormOnAfterSubmitForm)}",
            false,
            TimeSpan.FromSeconds(10));
    }

    private void CreateNewDirectoryFormOnAfterSubmitForm(string parentDirectoryAbsoluteFilePathString,
        string directoryName)
    {
        var localRefreshContextMenuTarget = ContextMenuEventDto.RefreshContextMenuTarget;

        _ = TaskModelManagerService.EnqueueTaskModelAsync(async cancellationToken =>
            {
                Directory.CreateDirectory(parentDirectoryAbsoluteFilePathString + directoryName);

                await localRefreshContextMenuTarget();

                Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
            },
            $"{nameof(CreateNewDirectoryFormOnAfterSubmitForm)}",
            false,
            TimeSpan.FromSeconds(10));
    }

    private void DeleteFileFormOnAfterSubmitForm(AbsoluteFilePathDotNet absoluteFilePath)
    {
        _ = TaskModelManagerService.EnqueueTaskModelAsync(async cancellationToken =>
            {
                if (absoluteFilePath.IsDirectory)
                {
                    Directory.Delete(absoluteFilePath.GetAbsoluteFilePathString(), true);
                    await ContextMenuEventDto.RefreshParentOfContextMenuTarget.Invoke();
                }
                else
                {
                    File.Delete(absoluteFilePath.GetAbsoluteFilePathString());
                    await ContextMenuEventDto.RefreshParentOfContextMenuTarget.Invoke();
                }

                Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
            },
            $"{nameof(DeleteFileFormOnAfterSubmitForm)}",
            false,
            TimeSpan.FromSeconds(10));
    }

    private bool AddProjectReferenceInputIsValidOverride(ImmutableArray<IAbsoluteFilePath> activeItems)
    {
        return activeItems[0].ExtensionNoPeriod == ExtensionNoPeriodFacts.C_SHARP_PROJECT;
    }
}