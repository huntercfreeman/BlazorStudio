using System.Collections.Immutable;
using BlazorALaCarte.Shared.Dropdown;
using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.BaseTypes;
using BlazorALaCarte.TreeView.Commands;
using BlazorALaCarte.TreeView.Services;
using BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorStudio.RazorLib.BalcTreeView.WatchWindowExample;

public partial class TextEditorDebugDisplay : FluxorComponent
{
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private IDropdownService DropdownService { get; set; } = null!;
    [Inject]
    private ITreeViewRenderers TreeViewRenderers { get; set; } = null!;

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationStateTask { get; set; }
    
    public static TreeViewStateKey TextEditorDebugTreeViewStateKey { get; } = TreeViewStateKey.NewTreeViewStateKey();
    public static DropdownKey WatchWindowContextMenuDropdownKey { get; } = DropdownKey.NewDropdownKey();
 
    private ITreeViewCommandParameter? _mostRecentTreeViewContextMenuCommandParameter;

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationStateTask is not null)
        {
            var authenticationState = await AuthenticationStateTask;
            
            if (!TreeViewService.TryGetTreeViewState(
                    TextEditorDebugTreeViewStateKey, 
                    out var treeViewState))
            {
                var rootDebugObject = new TextEditorDebugObjectWrap(
                    authenticationState,
                    typeof(AuthenticationState),
                    nameof(AuthenticationState),
                    true);
            
                var rootNode = new TreeViewReflection(
                    rootDebugObject,
                    true,
                    false,
                    TreeViewRenderers);

                TreeViewService.RegisterTreeViewState(new TreeViewState(
                    TextEditorDebugTreeViewStateKey,
                    rootNode,
                    rootNode,
                    ImmutableList<TreeViewNoType>.Empty));
            }
        }
        
        await base.OnInitializedAsync();
    }
    
    private async Task OnTreeViewContextMenuFunc(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _mostRecentTreeViewContextMenuCommandParameter = treeViewCommandParameter;
        
        DropdownService.AddActiveDropdownKey(
            WatchWindowContextMenuDropdownKey);
        
        await InvokeAsync(StateHasChanged);
    }
}