using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileSystemApi;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.ClassLib.Store.SolutionExplorerCase;
using BlazorStudio.ClassLib.Store.StartupProject;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.ClassLib.Templates;
using BlazorStudio.RazorLib.Forms;
using BlazorStudio.RazorLib.InputFile;
using BlazorStudio.RazorLib.SyntaxRootRender;
using BlazorStudio.RazorLib.TreeViewCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;

namespace BlazorStudio.RazorLib.Workspace;

public partial class WorkspaceExplorerMenuWrapperDisplay : ComponentBase
{
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IState<SolutionExplorerState> SolutionExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IState<BlazorStudio.ClassLib.Store.SolutionCase.SolutionState> SolutionStateWrap { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    public TreeViewWrapDisplay<IAbsoluteFilePath>.ContextMenuEventDto<IAbsoluteFilePath> ContextMenuEventDto { get; set; } = null!;

    private DialogKey _addProjectReferenceDialogKey = DialogKey.NewDialogKey();
    private DialogKey _syntaxRootDisplayDialogKey = DialogKey.NewDialogKey();

    private IEnumerable<MenuOptionRecord> GetMenuOptionRecords(
        TreeViewWrapDisplay<IAbsoluteFilePath>.ContextMenuEventDto<IAbsoluteFilePath> contextMenuEventDto)
    {
        var createNewEmptyFile = MenuOptionFacts.File
            .ConstructCreateNewEmptyFile(typeof(CreateNewFileForm),
                new Dictionary<string, object?>()
                {
                    {
                        nameof(CreateNewFileForm.ParentDirectory),
                        contextMenuEventDto.Item
                    },
                    {
                        nameof(CreateNewFileForm.OnAfterSubmitForm),
                        new Action<string, string>(CreateNewEmptyFileFormOnAfterSubmitForm)
                    },
                });

        var createNewDirectory = MenuOptionFacts.File
            .ConstructCreateNewDirectory(typeof(CreateNewDirectoryForm),
                new Dictionary<string, object?>()
                {
                    {
                        nameof(CreateNewFileForm.ParentDirectory),
                        contextMenuEventDto.Item
                    },
                    {
                        nameof(CreateNewFileForm.OnAfterSubmitForm),
                        new Action<string, string>(CreateNewDirectoryFormOnAfterSubmitForm)
                    },
                });

        var setActiveSolution = MenuOptionFacts.DotNet
            .SetActiveSolution(() => Dispatcher.Dispatch(new SetSolutionExplorerAction(contextMenuEventDto.Item, SequenceKey.NewSequenceKey())));

        List<MenuOptionRecord> menuOptionRecords = new();

        if (contextMenuEventDto.Item.IsDirectory)
        {
            menuOptionRecords.Add(createNewEmptyFile);
            menuOptionRecords.Add(createNewDirectory);
        }

        if (contextMenuEventDto.Item.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
        {
            menuOptionRecords.Add(setActiveSolution);
        }

        return menuOptionRecords.Any()
            ? menuOptionRecords
            : new[]
            {
                new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
                    "No Context Menu Options for this item",
                    ImmutableList<MenuOptionRecord>.Empty,
                    null,
                    MenuOptionKind.Read)
            };
    }

    private void CreateNewEmptyFileFormOnAfterSubmitForm(string parentDirectoryAbsoluteFilePathString,
        string fileName)
    {
#if DEBUG
        var localRefreshContextMenuTarget = ContextMenuEventDto.RefreshContextMenuTarget;

        _ = TaskModelManagerService.EnqueueTaskModelAsync(async (cancellationToken) =>
            {
                await File
                    .AppendAllTextAsync(parentDirectoryAbsoluteFilePathString + fileName,
                        string.Empty);

                await localRefreshContextMenuTarget();

                Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
            },
            $"{nameof(CreateNewEmptyFileFormOnAfterSubmitForm)}",
            false,
            TimeSpan.FromSeconds(10));
#endif
    }

    private void CreateNewDirectoryFormOnAfterSubmitForm(string parentDirectoryAbsoluteFilePathString,
        string directoryName)
    {
#if DEBUG
        _ = TaskModelManagerService.EnqueueTaskModelAsync(async (cancellationToken) =>
        {
            Directory.CreateDirectory(parentDirectoryAbsoluteFilePathString + directoryName);

            await ContextMenuEventDto.RefreshContextMenuTarget.Invoke();

            Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
        },
            $"{nameof(CreateNewDirectoryFormOnAfterSubmitForm)}",
            false,
            TimeSpan.FromSeconds(10));
#endif
    }

    private void OpenSyntaxRootDisplayDialog(IAbsoluteFilePath absoluteFilePath)
    {
        Task.Run(async () =>
        {
            SyntaxNode? targetSyntaxNode = null;

            var solutionState = SolutionStateWrap.Value;

            if (solutionState.SolutionWorkspace is null)
                return;

            foreach (Project project in solutionState.SolutionWorkspace.CurrentSolution.Projects)
            {
                foreach (Document document in project.Documents)
                {
                    if (document.FilePath?.Contains(absoluteFilePath.FilenameWithExtension) ?? false)
                    {
                        var syntax = await document.GetSyntaxTreeAsync();

                        targetSyntaxNode = await syntax.GetRootAsync();
                    }
                }
            }

            if (DialogStatesWrap.Value.List.All(x => x.DialogKey != _syntaxRootDisplayDialogKey))
            {
                var dialogRecord = new DialogRecord(
                    _syntaxRootDisplayDialogKey,
                    "Syntax Root Render",
                    typeof(SyntaxRootDisplay),
                    new Dictionary<string, object?>()
                    {
                        {
                            nameof(SyntaxRootDisplay.SyntaxNode),
                            targetSyntaxNode
                        }
                    }
                );

                Dispatcher.Dispatch(new RegisterDialogAction(dialogRecord));
            }
        });
    }

    private bool AddProjectReferenceInputIsValidOverride(ImmutableArray<IAbsoluteFilePath> activeItems)
    {
        return activeItems[0].ExtensionNoPeriod == ExtensionNoPeriodFacts.C_SHARP_PROJECT;
    }
}