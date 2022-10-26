using BlazorStudio.ClassLib.Errors;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.EditorCase;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.TextEditorResourceCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.RazorLib.TreeViewCase;
using BlazorTextEditor.RazorLib;
using BlazorTextEditor.RazorLib.TextEditor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.FolderExplorer;

public partial class FolderExplorer : FluxorComponent
{
    private Dimensions _fileDropdownDimensions = new()
    {
        DimensionsPositionKind = DimensionsPositionKind.Absolute,
        LeftCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = 5,
            },
        },
        TopCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.RootCharacterHeight,
                Value = 2.5,
            },
        },
    };

    private DropdownKey _fileDropdownKey = DropdownKey.NewDropdownKey();
    private TreeViewWrapKey _inputFileTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();

    private bool _isInitialized;
    private Func<Task> _mostRecentRefreshContextMenuTarget;
    private List<IAbsoluteFilePath> _rootAbsoluteFilePaths;
    private TreeViewWrap<IAbsoluteFilePath> _treeViewWrap = null!;
    private TreeViewWrapDisplay<IAbsoluteFilePath>? _treeViewWrapDisplay;
    private RichErrorModel? _workspaceStateWrapStateChangedRichErrorModel;
    [Inject]
    private IState<FolderExplorerState> FolderExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IState<TextEditorResourceState> TextEditorResourceStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;

    protected override void OnInitialized()
    {
        FolderExplorerStateWrap.StateChanged += WorkspaceStateWrap_StateChanged;

        base.OnInitialized();
    }

    private async void WorkspaceStateWrap_StateChanged(object? sender, EventArgs e)
    {
        var workspaceState = FolderExplorerStateWrap.Value;

        if (workspaceState.FolderAbsoluteFilePath is not null)
        {
            _isInitialized = false;
            _workspaceStateWrapStateChangedRichErrorModel = null;
            _rootAbsoluteFilePaths = null;

            await InvokeAsync(StateHasChanged);

            _treeViewWrap = new TreeViewWrap<IAbsoluteFilePath>(
                TreeViewWrapKey.NewTreeViewWrapKey());

            _ = TaskModelManagerService.EnqueueTaskModelAsync(async cancellationToken =>
                {
                    _rootAbsoluteFilePaths =
                        (await LoadAbsoluteFilePathChildrenAsync(workspaceState.FolderAbsoluteFilePath))
                        .ToList();

                    _isInitialized = true;

                    await InvokeAsync(StateHasChanged);

                    if (_treeViewWrapDisplay is not null) _treeViewWrapDisplay.Reload();
                },
                $"{nameof(WorkspaceStateWrap_StateChanged)}",
                false,
                TimeSpan.FromSeconds(10),
                exception =>
                {
                    _isInitialized = true;
                    _workspaceStateWrapStateChangedRichErrorModel = new RichErrorModel(
                        $"{nameof(WorkspaceStateWrap_StateChanged)}: {exception.Message}",
                        "TODO: Add a hint");

                    InvokeAsync(StateHasChanged);

                    return _workspaceStateWrapStateChangedRichErrorModel;
                });
        }
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

    private void FolderExplorerTreeViewOnEnterKeyDown(
        TreeViewKeyboardEventDto<IAbsoluteFilePath> treeViewKeyboardEventDto)
    {
        if (!treeViewKeyboardEventDto.Item.IsDirectory)
        {
            _ = Task.Run(async () =>
            {
                var content = await FileSystemProvider
                    .ReadFileAsync(
                        treeViewKeyboardEventDto.Item);

                var textEditor = new TextEditorBase(
                    content,
                    null,
                    null,
                    null);

                Dispatcher.Dispatch(new SetTextEditorResourceStateAction(
                    textEditor.Key,
                    treeViewKeyboardEventDto.Item));

                await textEditor.ApplySyntaxHighlightingAsync();

                TextEditorService
                    .RegisterTextEditor(textEditor);

                Dispatcher.Dispatch(
                    new SetActiveTextEditorKeyAction(textEditor.Key));
            });
        }
        else
            treeViewKeyboardEventDto.ToggleIsExpanded.Invoke();
    }

    private void FolderExplorerTreeViewOnSpaceKeyDown(
        TreeViewKeyboardEventDto<IAbsoluteFilePath> treeViewKeyboardEventDto)
    {
        treeViewKeyboardEventDto.ToggleIsExpanded.Invoke();
    }

    private void FolderExplorerTreeViewOnDoubleClick(TreeViewMouseEventDto<IAbsoluteFilePath> treeViewMouseEventDto)
    {
        if (!treeViewMouseEventDto.Item.IsDirectory)
        {
            _ = Task.Run(async () =>
            {
                var content = await FileSystemProvider
                    .ReadFileAsync(
                        treeViewMouseEventDto.Item);

                var textEditor = new TextEditorBase(
                    content,
                    null,
                    null,
                    null);

                Dispatcher.Dispatch(new SetTextEditorResourceStateAction(
                    textEditor.Key,
                    treeViewMouseEventDto.Item));

                await textEditor.ApplySyntaxHighlightingAsync();

                TextEditorService
                    .RegisterTextEditor(textEditor);

                Dispatcher.Dispatch(
                    new SetActiveTextEditorKeyAction(textEditor.Key));
            });
        }
        else
            treeViewMouseEventDto.ToggleIsExpanded.Invoke();
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
        FolderExplorerStateWrap.StateChanged -= WorkspaceStateWrap_StateChanged;

        base.Dispose(disposing);
    }
}