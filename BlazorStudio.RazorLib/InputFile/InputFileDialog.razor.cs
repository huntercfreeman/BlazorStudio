using System.Collections.Immutable;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.ThemeCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileDialog : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private bool _isInitialized;
    private TreeViewWrapKey _inputFileTreeViewKey = TreeViewWrapKey.NewTreeViewWrapKey();
    private TreeViewWrap<IAbsoluteFilePath> _treeViewWrap = null!;
    private List<IAbsoluteFilePath> _rootAbsoluteFilePaths;

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
}