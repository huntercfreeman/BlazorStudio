using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.Errors;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.FileSystemApi.MemoryMapped;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.RazorLib.ContextCase;
using BlazorStudio.RazorLib.TreeViewCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.FolderExplorer;

public partial class FolderExplorer : FluxorComponent
{
    [Inject]
    private IState<FolderExplorerState> WorkspaceStateWrap { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;

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

    private async void WorkspaceStateWrap_StateChanged(object? sender, EventArgs e)
    {
        var workspaceState = WorkspaceStateWrap.Value;

        if (workspaceState.FolderAbsoluteFilePath is not null)
        {
            _isInitialized = false;
            _workspaceStateWrapStateChangedRichErrorModel = null;
            _rootAbsoluteFilePaths = null;

            await InvokeAsync(StateHasChanged);

            _treeViewWrap = new TreeViewWrap<IAbsoluteFilePath>(
                TreeViewWrapKey.NewTreeViewWrapKey());

            _ = TaskModelManagerService.EnqueueTaskModelAsync(async (cancellationToken) =>
                {
                    _rootAbsoluteFilePaths = (await LoadAbsoluteFilePathChildrenAsync(workspaceState.FolderAbsoluteFilePath))
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
            .OrderBy(x => x)
            .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, true))
            .ToList();

        var childFileAbsolutePaths = Directory
            .GetFiles(absoluteFilePath.GetAbsoluteFilePathString())
            .OrderBy(x => x)
            .Select(x => (IAbsoluteFilePath)new AbsoluteFilePath(x, false))
            .ToList();

        return childDirectoryAbsolutePaths
            .Union(childFileAbsolutePaths);
    }

    private void WorkspaceExplorerTreeViewOnEnterKeyDown(TreeViewKeyboardEventDto<IAbsoluteFilePath> treeViewKeyboardEventDto)
    {
        if (!treeViewKeyboardEventDto.Item.IsDirectory)
        {
            var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

            Dispatcher.Dispatch(
                new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey,
                    treeViewKeyboardEventDto.Item,
                    FileSystemProvider,
                    CancellationToken.None)
            );
        }
        else
        {
            treeViewKeyboardEventDto.ToggleIsExpanded.Invoke();
        }
    }

    private void WorkspaceExplorerTreeViewOnSpaceKeyDown(TreeViewKeyboardEventDto<IAbsoluteFilePath> treeViewKeyboardEventDto)
    {
        treeViewKeyboardEventDto.ToggleIsExpanded.Invoke();
    }

    private void WorkspaceExplorerTreeViewOnDoubleClick(TreeViewMouseEventDto<IAbsoluteFilePath> treeViewMouseEventDto)
    {
        if (!treeViewMouseEventDto.Item.IsDirectory)
        {
            var plainTextEditorKey = PlainTextEditorKey.NewPlainTextEditorKey();

            Dispatcher.Dispatch(
                new ConstructTokenizedPlainTextEditorRecordAction(plainTextEditorKey,
                    treeViewMouseEventDto.Item,
                    FileSystemProvider,
                    CancellationToken.None)
            );
        }
        else
        {
            treeViewMouseEventDto.ToggleIsExpanded.Invoke();
        }
    }

    private bool GetIsExpandable(IAbsoluteFilePath absoluteFilePath)
    {
        return absoluteFilePath.IsDirectory;
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