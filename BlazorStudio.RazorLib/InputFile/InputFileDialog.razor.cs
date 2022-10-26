using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.RazorLib.Forms;
using BlazorStudio.RazorLib.TreeViewCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileDialog : ComponentBase
{
    private TreeViewWrapKey _inputFileTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();

    private bool _isInitialized;
    private List<IAbsoluteFilePath>? _rootAbsoluteFilePaths;
    private TreeViewWrap<IAbsoluteFilePath> _treeViewWrap = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    [Parameter]
    public Action<(IAbsoluteFilePath absoluteFilePath, Action? toggleIsExpanded)>? OnEnterKeyDownOverride { get; set; }
    [Parameter]
    public Action<(IAbsoluteFilePath absoluteFilePath, Action? toggleIsExpanded)>? OnSpaceKeyDownOverride { get; set; }
    [Parameter]
    public Action<(IAbsoluteFilePath absoluteFilePath, Action? toggleIsExpanded, MouseEventArgs mouseEventArgs)>?
        OnDoubleClickOverride { get; set; }
    [Parameter]
    public Action<TreeViewContextMenuEventDto<IAbsoluteFilePath>>? ChooseContextMenuOption { get; set; }
    [Parameter]
    public bool ShowFooter { get; set; } = true;
    [Parameter]
    public Func<ImmutableArray<IAbsoluteFilePath>, bool>? IsValidSelectionOverrideFunc { get; set; }
    [Parameter]
    public string? InvalidSelectionTextOverride { get; set; }
    [Parameter]
    public Action<ImmutableArray<IAbsoluteFilePath>>? ConfirmOnClickOverrideAction { get; set; }

    private string BodyCssClassString => ShowFooter
        ? "bstudio_input-file-dialog-body-show-footer"
        : "bstudio_input-file-dialog-body-hide-footer";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // var rootAbsoluteFilePath =
            //     new AbsoluteFilePath(
            //         System.IO.Path.DirectorySeparatorChar.ToString(),
            //         true);

            var rootAbsoluteFilePath =
                new AbsoluteFilePath(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    true);

            _treeViewWrap = new TreeViewWrap<IAbsoluteFilePath>(
                TreeViewWrapKey.NewTreeViewWrapKey());

            _rootAbsoluteFilePaths = (await LoadAbsoluteFilePathChildrenAsync(rootAbsoluteFilePath))
                .ToList();

            _isInitialized = true;

            await InvokeAsync(StateHasChanged);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private Task<IEnumerable<IAbsoluteFilePath>> LoadAbsoluteFilePathChildrenAsync(IAbsoluteFilePath absoluteFilePath)
    {
        if (!absoluteFilePath.IsDirectory)
            return Task.FromResult<IEnumerable<IAbsoluteFilePath>>(Array.Empty<IAbsoluteFilePath>());

        var childDirectoryAbsolutePaths = Directory
            .GetDirectories(absoluteFilePath.GetAbsoluteFilePathString())
            .OrderBy(x => x)
            .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, true))
            .ToList();

        var childFileAbsolutePaths = Directory
            .GetFiles(absoluteFilePath.GetAbsoluteFilePathString())
            .OrderBy(x => x)
            .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, false))
            .ToList();

        return Task.FromResult(childDirectoryAbsolutePaths
            .Union(childFileAbsolutePaths));
    }

    private void InputFileTreeViewOnEnterKeyDown(TreeViewKeyboardEventDto<IAbsoluteFilePath> treeViewKeyboardEventDto)
    {
        if (OnEnterKeyDownOverride is not null)
        {
            OnEnterKeyDownOverride.Invoke((treeViewKeyboardEventDto.Item, treeViewKeyboardEventDto.ToggleIsExpanded));
            return;
        }

        if (treeViewKeyboardEventDto.Item.IsDirectory)
        {
            Dispatcher.Dispatch(new SetFolderExplorerAction(treeViewKeyboardEventDto.Item));
            Dispatcher.Dispatch(new DisposeDialogAction(DialogRecord));
        }
    }

    private void InputFileTreeViewOnSpaceKeyDown(TreeViewKeyboardEventDto<IAbsoluteFilePath> treeViewKeyboardEventDto)
    {
        if (OnSpaceKeyDownOverride is not null)
        {
            OnSpaceKeyDownOverride.Invoke((treeViewKeyboardEventDto.Item, treeViewKeyboardEventDto.ToggleIsExpanded));
            return;
        }

        if (treeViewKeyboardEventDto.Item.IsDirectory) 
            treeViewKeyboardEventDto.ToggleIsExpanded?.Invoke();
    }

    private void InputFileTreeViewOnDoubleClick(TreeViewMouseEventDto<IAbsoluteFilePath> treeViewMouseEventDto)
    {
        if (OnDoubleClickOverride is not null)
        {
            OnDoubleClickOverride.Invoke((treeViewMouseEventDto.Item, treeViewMouseEventDto.ToggleIsExpanded,
                treeViewMouseEventDto.MouseEventArgs));
            return;
        }

        treeViewMouseEventDto.ToggleIsExpanded?.Invoke();
    }

    private bool GetIsExpandable(IAbsoluteFilePath absoluteFilePath)
    {
        return absoluteFilePath.IsDirectory;
    }

    private void ConfirmOnClick(ImmutableArray<IAbsoluteFilePath> absoluteFilePaths)
    {
        if (ConfirmOnClickOverrideAction is not null)
            ConfirmOnClickOverrideAction.Invoke(absoluteFilePaths);
        else if (absoluteFilePaths.Any())
        {
            Dispatcher.Dispatch(new SetFolderExplorerAction(absoluteFilePaths[0]));
            Dispatcher.Dispatch(new DisposeDialogAction(DialogRecord));
        }
    }

    private void CancelOnClick()
    {
        Dispatcher.Dispatch(new DisposeDialogAction(DialogRecord));
    }

    private IEnumerable<MenuOptionRecord> GetMenuOptionRecords(
        TreeViewContextMenuEventDto<IAbsoluteFilePath> contextMenuEventDto)
    {
        List<MenuOptionRecord> menuOptionRecords = new();

        if (ChooseContextMenuOption is not null)
        {
            var setActiveSelection = new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
                "Set Selection",
                ImmutableList<MenuOptionRecord>.Empty,
                () => ChooseContextMenuOption(contextMenuEventDto),
                MenuOptionKind.Update);

            menuOptionRecords.Add(setActiveSelection);
        }

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
                        (string str1, string str2, bool _) =>
                            CreateNewFileFormOnAfterSubmitForm(
                                str1,
                                str2,
                                contextMenuEventDto)
                    },
                    {
                        nameof(CreateNewFileForm.OnAfterCancelForm),
                        new Action(() =>
                        {
                            Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());

                            _ = Task.Run(async () => await contextMenuEventDto.FocusAfterTarget.FocusAsync());
                        })
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
                        (string str1, string str2) =>
                            CreateNewDirectoryFormOnAfterSubmitForm(
                                str1,
                                str2,
                                contextMenuEventDto)
                    },
                    {
                        nameof(CreateNewDirectoryForm.OnAfterCancelForm),
                        new Action(() =>
                        {
                            Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());

                            _ = Task.Run(async () => await contextMenuEventDto.FocusAfterTarget.FocusAsync());
                        })
                    },
                });

        if (contextMenuEventDto.Item.IsDirectory)
        {
            menuOptionRecords.Add(createNewEmptyFile);
            menuOptionRecords.Add(createNewDirectory);
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

    private void CreateNewFileFormOnAfterSubmitForm(string parentDirectoryAbsoluteFilePathString,
        string fileName,
        TreeViewContextMenuEventDto<IAbsoluteFilePath> contextMenuEventDto)
    {
        _ = TaskModelManagerService.EnqueueTaskModelAsync(async cancellationToken =>
            {
                await File
                    .AppendAllTextAsync(parentDirectoryAbsoluteFilePathString + fileName,
                        string.Empty);

                await contextMenuEventDto.RefreshContextMenuTarget.Invoke();
                await contextMenuEventDto.FocusAfterTarget.FocusAsync();

                Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
            },
            $"{nameof(CreateNewFileFormOnAfterSubmitForm)}",
            false,
            TimeSpan.FromSeconds(10));
    }

    private void CreateNewDirectoryFormOnAfterSubmitForm(string parentDirectoryAbsoluteFilePathString,
        string directoryName,
        TreeViewContextMenuEventDto<IAbsoluteFilePath> contextMenuEventDto)
    {
        _ = TaskModelManagerService.EnqueueTaskModelAsync(async cancellationToken =>
            {
                Directory.CreateDirectory(parentDirectoryAbsoluteFilePathString + directoryName);

                await contextMenuEventDto.RefreshContextMenuTarget.Invoke();
                await contextMenuEventDto.FocusAfterTarget.FocusAsync();

                Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
            },
            $"{nameof(CreateNewDirectoryFormOnAfterSubmitForm)}",
            false,
            TimeSpan.FromSeconds(10));
    }
}