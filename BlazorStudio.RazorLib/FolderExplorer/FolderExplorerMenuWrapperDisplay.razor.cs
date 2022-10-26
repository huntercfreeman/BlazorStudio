using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Sequence;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using BlazorStudio.ClassLib.Store.SolutionExplorerCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.RazorLib.Forms;
using BlazorStudio.RazorLib.TreeViewCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.FolderExplorer;

public partial class FolderExplorerMenuWrapperDisplay : ComponentBase
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
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    public TreeViewContextMenuEventDto<IAbsoluteFilePath> ContextMenuEventDto { get; set; } = null!;

    private IEnumerable<MenuOptionRecord> GetMenuOptionRecords(
        TreeViewContextMenuEventDto<IAbsoluteFilePath> contextMenuEventDto)
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
                        new Action<string, string>(CreateNewEmptyFileFormOnAfterSubmitForm)
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
                        new Action<IAbsoluteFilePath>(DeleteFileFormOnAfterSubmitForm)
                    },
                    {
                        nameof(DeleteFileForm.OnAfterCancelForm),
                        new Action(() => Dispatcher.Dispatch(new ClearActiveDropdownKeysAction()))
                    },
                });

        var absoluteFilePathDotNet =
            new AbsoluteFilePathDotNet(contextMenuEventDto.Item.GetAbsoluteFilePathString(), false, null);

        var setActiveSolution = MenuOptionFacts.DotNet
            .SetActiveSolution(() =>
                Dispatcher.Dispatch(new SetSolutionExplorerAction(absoluteFilePathDotNet,
                    SequenceKey.NewSequenceKey())));

        List<MenuOptionRecord> menuOptionRecords = new();

        if (contextMenuEventDto.Item.IsDirectory)
        {
            menuOptionRecords.Add(createNewEmptyFile);
            menuOptionRecords.Add(createNewDirectory);
        }

        menuOptionRecords.Add(createDeleteFile);

        if (contextMenuEventDto.Item.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
            menuOptionRecords.Add(setActiveSolution);

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
        string fileName)
    {
        var localRefreshContextMenuTarget = ContextMenuEventDto.RefreshContextMenuTarget;

        _ = TaskModelManagerService.EnqueueTaskModelAsync(async cancellationToken =>
            {
                await File
                    .AppendAllTextAsync(parentDirectoryAbsoluteFilePathString + fileName,
                        string.Empty, 
                        cancellationToken);

                await localRefreshContextMenuTarget();

                Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
            },
            $"{nameof(CreateNewEmptyFileFormOnAfterSubmitForm)}",
            false,
            TimeSpan.FromSeconds(10));
    }

    private void CreateNewDirectoryFormOnAfterSubmitForm(string parentDirectoryAbsoluteFilePathString,
        string directoryName)
    {
        _ = TaskModelManagerService.EnqueueTaskModelAsync(async _ =>
            {
                Directory.CreateDirectory(parentDirectoryAbsoluteFilePathString + directoryName);

                await ContextMenuEventDto.RefreshContextMenuTarget.Invoke();

                Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
            },
            $"{nameof(CreateNewDirectoryFormOnAfterSubmitForm)}",
            false,
            TimeSpan.FromSeconds(10));
    }

    private void DeleteFileFormOnAfterSubmitForm(IAbsoluteFilePath absoluteFilePath)
    {
        _ = TaskModelManagerService.EnqueueTaskModelAsync(async _ =>
            {
                if (absoluteFilePath.IsDirectory)
                    Directory.Delete(absoluteFilePath.GetAbsoluteFilePathString(), true);
                else
                    File.Delete(absoluteFilePath.GetAbsoluteFilePathString());

                if (ContextMenuEventDto.RefreshParentOfContextMenuTarget is not null)
                    await ContextMenuEventDto.RefreshParentOfContextMenuTarget.Invoke();

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