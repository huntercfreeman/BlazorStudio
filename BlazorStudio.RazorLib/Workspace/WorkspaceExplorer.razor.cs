using System.Collections.Immutable;
using BlazorStudio.ClassLib.Errors;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.ClassLib.Store.WorkspaceCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.RazorLib.Forms;
using BlazorStudio.RazorLib.TreeViewCase;
using BlazorStudio.Shared.FileSystem.Classes;
using BlazorStudio.Shared.FileSystem.Interfaces;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorStudio.RazorLib.Workspace;

public partial class WorkspaceExplorer : FluxorComponent, IDisposable
{
    [Inject]
    private IState<WorkspaceState> WorkspaceStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;

    private void LoadFile(InputFileChangeEventArgs e)
    {
    }

    private bool _isInitialized;
    private TreeViewWrapKey _inputFileTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private TreeViewWrap<IAbsoluteFilePath> _treeViewWrap = null!;
    private List<IAbsoluteFilePath> _rootAbsoluteFilePaths;
    private RichErrorModel? _workspaceStateWrapStateChangedRichErrorModel;
    private TreeViewWrapDisplay<IAbsoluteFilePath>? _treeViewWrapDisplay;
    private Func<Task> _mostRecentRefreshContextMenuTarget;
    
    private Dimensions _fileDropdownDimensions = new()
    {
        DimensionsPositionKind = DimensionsPositionKind.Absolute,
        LeftCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = 5
            }
        },
        TopCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.RootCharacterHeight,
                Value = 2.5
            }
        },
    };

    private DropdownKey _fileDropdownKey = DropdownKey.NewDropdownKey();

    protected override void OnInitialized()
    {
        WorkspaceStateWrap.StateChanged += WorkspaceStateWrap_StateChanged;

        base.OnInitialized();
    }

    private async void WorkspaceStateWrap_StateChanged(object? sender, EventArgs e)
    {
        var workspaceState = WorkspaceStateWrap.Value;

        if (workspaceState.WorkspaceAbsoluteFilePath is not null)
        {
            _isInitialized = false;
            _workspaceStateWrapStateChangedRichErrorModel = null;
            _rootAbsoluteFilePaths = null;

            await InvokeAsync(StateHasChanged);

            _treeViewWrap = new TreeViewWrap<IAbsoluteFilePath>(
                TreeViewWrapKey.NewTreeViewWrapKey());

            _ = TaskModelManagerService.EnqueueTaskModelAsync(async (cancellationToken) =>
                {
                    _rootAbsoluteFilePaths = (await LoadAbsoluteFilePathChildrenAsync(workspaceState.WorkspaceAbsoluteFilePath))
                        .ToList();

                    _isInitialized = true;

                    await InvokeAsync(StateHasChanged);

                    if (_treeViewWrapDisplay is not null)
                    {
                        _treeViewWrapDisplay.Reload();
                    }
                },
                $"{nameof(WorkspaceStateWrap_StateChanged)}",
                false,
                TimeSpan.FromSeconds(10),
                exception =>
                {
                    _isInitialized = true;
                    _workspaceStateWrapStateChangedRichErrorModel = new RichErrorModel(
                        $"{nameof(WorkspaceStateWrap_StateChanged)}: {exception.Message}",
                        $"TODO: Add a hint");

                    InvokeAsync(StateHasChanged);

                    return _workspaceStateWrapStateChangedRichErrorModel;
                });
        }
    }

    private async Task<IEnumerable<IAbsoluteFilePath>> LoadAbsoluteFilePathChildrenAsync(IAbsoluteFilePath absoluteFilePath)
    {
        if (!absoluteFilePath.IsDirectory)
        {
            return Array.Empty<IAbsoluteFilePath>();
        }

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
    }

    private void WorkspaceExplorerTreeViewOnEnterKeyDown(IAbsoluteFilePath absoluteFilePath)
    {
        if (!absoluteFilePath.IsDirectory)
            Dispatcher.Dispatch(new SetOpenedAbsoluteFilePathAction(absoluteFilePath));
    }

    private void WorkspaceExplorerTreeViewOnSpaceKeyDown(IAbsoluteFilePath absoluteFilePath)
    {
    }

    private bool GetIsExpandable(IAbsoluteFilePath absoluteFilePath)
    {
        return absoluteFilePath.IsDirectory;
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
            : new[]
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
    }

    private void CreateNewDirectoryFormOnAfterSubmitForm(string parentDirectoryAbsoluteFilePathString,
        string directoryName)
    {
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
    }

    private void DispatchAddActiveDropdownKeyActionOnClick(DropdownKey fileDropdownKey)
    {
        Dispatcher.Dispatch(new AddActiveDropdownKeyAction(fileDropdownKey));
    }

    protected override void Dispose(bool disposing)
    {
        WorkspaceStateWrap.StateChanged -= WorkspaceStateWrap_StateChanged;


        base.Dispose(disposing);
    }
}