using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.ClassLib.Store.WorkspaceCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.RazorLib.Forms;
using BlazorStudio.RazorLib.TreeViewCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.DropdownCase;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileDialog : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private bool _isInitialized;
    private TreeViewWrapKey _inputFileTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private TreeViewWrap<IAbsoluteFilePath> _treeViewWrap = null!;
    private List<IAbsoluteFilePath> _rootAbsoluteFilePaths;
    private Func<Task> _mostRecentRefreshContextMenuTarget;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var rootAbsoluteFilePath =
                new AbsoluteFilePath(
                    System.IO.Path.DirectorySeparatorChar.ToString(),
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

    private async Task<IEnumerable<IAbsoluteFilePath>> LoadAbsoluteFilePathChildrenAsync(IAbsoluteFilePath absoluteFilePath)
    {
        if (!absoluteFilePath.IsDirectory)
        {
            return Array.Empty<IAbsoluteFilePath>();
        }

#if DEBUG
        var childDirectoryAbsolutePaths = Directory
            .GetDirectories(absoluteFilePath.GetAbsoluteFilePathString())
            .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, true))
            .ToList();

        var childFileAbsolutePaths = Directory
            .GetFiles(absoluteFilePath.GetAbsoluteFilePathString())
            .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, false))
            .ToList();

        return childDirectoryAbsolutePaths
            .Union(childFileAbsolutePaths);
#else
        var childDirectoryAbsolutePaths = new string[]
            {
                "Dir1",
                "Dir2",
                "Dir3",
            }
            .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, true))
            .ToList();

        var childFileAbsolutePaths = new string[]
            {
                "File1",
                "File2",
                "File3",
            }
            .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, false))
            .ToList();

        return childDirectoryAbsolutePaths
            .Union(childFileAbsolutePaths);
#endif
    }

    private void InputFileTreeViewOnEnterKeyDown(IAbsoluteFilePath absoluteFilePath, Action toggleIsExpanded)
    {
        if (absoluteFilePath.IsDirectory)
        {
            Dispatcher.Dispatch(new SetWorkspaceAction(absoluteFilePath));
            Dispatcher.Dispatch(new DisposeDialogAction(DialogRecord));
        }
    }

    private void InputFileTreeViewOnSpaceKeyDown(IAbsoluteFilePath absoluteFilePath, Action toggleIsExpanded)
    {
        if (absoluteFilePath.IsDirectory)
        {
            toggleIsExpanded.Invoke();
        }
    }
    
    private void InputFileTreeViewOnDoubleClick(IAbsoluteFilePath absoluteFilePath, Action toggleIsExpanded, MouseEventArgs mouseEventArgs)
    {
        toggleIsExpanded.Invoke();
    }

    private bool GetIsExpandable(IAbsoluteFilePath absoluteFilePath)
    {
        return absoluteFilePath.IsDirectory;
    }
    
    private void ConfirmOnClick(ImmutableArray<IAbsoluteFilePath> absoluteFilePaths)
    {
        if (absoluteFilePaths.Any())
        {
            Dispatcher.Dispatch(new SetWorkspaceAction(absoluteFilePaths[0]));
            Dispatcher.Dispatch(new DisposeDialogAction(DialogRecord));
        }
    }
    
    private void CancelOnClick()
    {
        Dispatcher.Dispatch(new DisposeDialogAction(DialogRecord));
    }

    private IEnumerable<MenuOptionRecord> GetMenuOptionRecords(
        TreeViewWrapDisplay<IAbsoluteFilePath>.ContextMenuEventDto<IAbsoluteFilePath> contextMenuEventDto)
    {
        var createNewFile = MenuOptionFacts.File
            .ConstructCreateNewFile(typeof(CreateNewFileForm),
                new Dictionary<string, object?>()
                {
                    {
                        nameof(CreateNewFileForm.ParentDirectory),
                        contextMenuEventDto.Item
                    },
                    {
                        nameof(CreateNewFileForm.OnAfterSubmitForm),
                        new Action<string, string>(CreateNewFileFormOnAfterSubmitForm)
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

        _mostRecentRefreshContextMenuTarget = contextMenuEventDto.RefreshContextMenuTarget;

        List<MenuOptionRecord> menuOptionRecords = new();

        if (contextMenuEventDto.Item.IsDirectory)
        {
            menuOptionRecords.Add(createNewFile);
            menuOptionRecords.Add(createNewDirectory);
        }

        return menuOptionRecords.Any()
            ? menuOptionRecords
            : new []
            {
                new MenuOptionRecord(MenuOptionKey.NewMenuOptionKey(),
                    "No Context Menu Options for this item",
                    ImmutableList<MenuOptionRecord>.Empty, 
                    null)
            };
    }
    
    private void CreateNewFileFormOnAfterSubmitForm(string parentDirectoryAbsoluteFilePathString, 
        string fileName)
    {
#if DEBUG
        var localRefreshContextMenuTarget = _mostRecentRefreshContextMenuTarget;

        _ = TaskModelManagerService.EnqueueTaskModelAsync(async (cancellationToken) =>
            {
                await File
                    .AppendAllTextAsync(parentDirectoryAbsoluteFilePathString + fileName, 
                        string.Empty);

                await localRefreshContextMenuTarget();

                Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
            },
            $"{nameof(CreateNewFileFormOnAfterSubmitForm)}",
            false,
            TimeSpan.FromSeconds(10));
#endif
    }

    private void CreateNewDirectoryFormOnAfterSubmitForm(string parentDirectoryAbsoluteFilePathString, 
        string directoryName)
    {
#if DEBUG
        var localRefreshContextMenuTarget = _mostRecentRefreshContextMenuTarget;

        _ = TaskModelManagerService.EnqueueTaskModelAsync(async (cancellationToken) =>
            {
                Directory.CreateDirectory(parentDirectoryAbsoluteFilePathString + directoryName);

                await localRefreshContextMenuTarget();

                Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
            },
            $"{nameof(CreateNewDirectoryFormOnAfterSubmitForm)}",
            false,
            TimeSpan.FromSeconds(10));
#endif
    }
}