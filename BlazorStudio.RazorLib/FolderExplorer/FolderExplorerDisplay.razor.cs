using System.Collections.Immutable;
using BlazorCommon.RazorLib.ComponentRenderers;
using BlazorCommon.RazorLib.Store.AccountCase;
using BlazorCommon.RazorLib.Store.ApplicationOptions;
using BlazorCommon.RazorLib.Store.DropdownCase;
using BlazorCommon.RazorLib.TreeView;
using BlazorCommon.RazorLib.TreeView.Commands;
using BlazorCommon.RazorLib.TreeView.TreeViewClasses;
using BlazorStudio.ClassLib.ComponentRenderers;
using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Store.FolderExplorerCase;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.Store.TerminalCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using BlazorStudio.RazorLib.FolderExplorer.Classes;
using BlazorStudio.RazorLib.FolderExplorer.InternalComponents;
using BlazorStudio.RazorLib.InputFile.InternalComponents;
using BlazorStudio.RazorLib.SolutionExplorer;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.FolderExplorer;

public partial class FolderExplorerDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<FolderExplorerState> FolderExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IState<TerminalSessionsState> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IState<AccountState> AccountStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IBlazorStudioComponentRenderers BlazorStudioComponentRenderers { get; set; } = null!;
    [Inject]
    private ClassLib.Menu.ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    public static readonly TreeViewStateKey TreeViewFolderExplorerContentStateKey =
        TreeViewStateKey.NewTreeViewStateKey();

    private FolderExplorerTreeViewMouseEventHandler _folderExplorerTreeViewMouseEventHandler = null!;
    private FolderExplorerTreeViewKeyboardEventHandler _folderExplorerTreeViewKeyboardEventHandler = null!;
    private ITreeViewCommandParameter? _mostRecentTreeViewCommandParameter;

    private TreeViewNodeKey _previousRootTreeViewNodeKey = TreeViewNodeKey.Empty;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsStateWrap.Value.Options.IconSizeInPixels.GetValueOrDefault() *
        (2.0 / 3.0));

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationStateTask is not null)
        {
            var authenticationState = await AuthenticationStateTask;

            var subClaim = authenticationState.User.FindFirst(
                AccountState.SUB_CLAIM_NAME);

            if (subClaim is null)
                return;
            
            Dispatcher.Dispatch(new AccountState.AccountStateWithAction(
                inAccountState => inAccountState with
                {
                    GroupName = subClaim.Value
                })); 

            var folderExplorerState = FolderExplorerStateWrap.Value;

            var treeViewStateFound = TreeViewService.TryGetTreeViewState(
                TreeViewFolderExplorerContentStateKey,
                out var treeViewState);

            if (treeViewStateFound &&
                treeViewState is not null &&
                _previousRootTreeViewNodeKey != treeViewState.RootNode.TreeViewNodeKey)
            {
                FolderExplorerStateWrapOnStateChanged(null, EventArgs.Empty);
            }
            else
            {
                if (folderExplorerState.AbsoluteFilePath is not null)
                    await SetFolderExplorerTreeViewRootAsync(folderExplorerState.AbsoluteFilePath);
            }
        }

        await base.OnInitializedAsync();
    }

    protected override void OnInitialized()
    {
        FolderExplorerStateWrap.StateChanged += FolderExplorerStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged += AppOptionsStateWrapOnStateChanged;

        _folderExplorerTreeViewMouseEventHandler = new FolderExplorerTreeViewMouseEventHandler(
            Dispatcher,
            TextEditorService,
            BlazorStudioComponentRenderers,
            FileSystemProvider,
            TreeViewService);

        _folderExplorerTreeViewKeyboardEventHandler = new FolderExplorerTreeViewKeyboardEventHandler(
            TerminalSessionsStateWrap,
            CommonMenuOptionsFactory,
            BlazorStudioComponentRenderers,
            FileSystemProvider,
            Dispatcher,
            TreeViewService,
            TextEditorService);

        base.OnInitialized();
    }

    private async void FolderExplorerStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        var folderExplorerState = FolderExplorerStateWrap.Value;

        if (folderExplorerState.AbsoluteFilePath is not null)
            await SetFolderExplorerTreeViewRootAsync(folderExplorerState.AbsoluteFilePath);

        await InvokeAsync(StateHasChanged);
    }

    private async void AppOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task SetFolderExplorerTreeViewRootAsync(IAbsoluteFilePath absoluteFilePath)
    {
        var rootNode = new TreeViewAbsoluteFilePath(
            absoluteFilePath,
            BlazorStudioComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            true);

        _previousRootTreeViewNodeKey = rootNode.TreeViewNodeKey;

        await rootNode.LoadChildrenAsync();

        if (!TreeViewService.TryGetTreeViewState(
                TreeViewFolderExplorerContentStateKey,
                out var treeViewState))
        {
            TreeViewService.RegisterTreeViewState(new TreeViewState(
                TreeViewFolderExplorerContentStateKey,
                rootNode,
                rootNode,
                ImmutableList<TreeViewNoType>.Empty));
        }
        else
        {
            TreeViewService.SetRoot(
                TreeViewFolderExplorerContentStateKey,
                rootNode);

            TreeViewService.SetActiveNode(
                TreeViewFolderExplorerContentStateKey,
                rootNode);
        }
    }

    private async Task OnTreeViewContextMenuFunc(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _mostRecentTreeViewCommandParameter = treeViewCommandParameter;

        Dispatcher.Dispatch(
            new DropdownsState.AddActiveAction(
                FolderExplorerContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        FolderExplorerStateWrap.StateChanged -= FolderExplorerStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
    }
}