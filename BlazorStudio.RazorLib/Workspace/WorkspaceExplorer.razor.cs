using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.ClassLib.Store.WorkspaceCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorStudio.RazorLib.Workspace;

public partial class WorkspaceExplorer : FluxorComponent, IDisposable
{
    [Inject]
    private IState<WorkspaceState> WorkspaceStateWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;

    private void LoadFile(InputFileChangeEventArgs e)
    {
    }

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private bool _isInitialized;
    private TreeViewWrapKey _inputFileTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private TreeViewWrap<IAbsoluteFilePath> _treeViewWrap = null!;
    private List<IAbsoluteFilePath> _rootAbsoluteFilePaths;

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
            _treeViewWrap = new TreeViewWrap<IAbsoluteFilePath>(
                TreeViewWrapKey.NewTreeViewWrapKey());

            _rootAbsoluteFilePaths = (await LoadAbsoluteFilePathChildrenAsync(workspaceState.WorkspaceAbsoluteFilePath))
                .ToList();

            _isInitialized = true;

            await InvokeAsync(StateHasChanged);
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

    private void InputFileTreeViewOnEnterKeyDown(IAbsoluteFilePath absoluteFilePath)
    {
    }

    private void InputFileTreeViewOnSpaceKeyDown(IAbsoluteFilePath absoluteFilePath)
    {
    }

    private bool GetIsExpandable(IAbsoluteFilePath absoluteFilePath)
    {
        return absoluteFilePath.IsDirectory;
    }

    protected override void Dispose(bool disposing)
    {
        WorkspaceStateWrap.StateChanged -= WorkspaceStateWrap_StateChanged;


        base.Dispose(disposing);
    }
}