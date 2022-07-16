using System.Collections.Immutable;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PlainTextEditor.ClassLib.Keyboard;

namespace BlazorStudio.RazorLib.TreeView;

public partial class TreeViewDisplay<T>
    where T : class
{
    [Inject]
    private IStateSelection<TreeViewWrapStates, ITreeViewWrap?> TreeViewWrapStateSelection { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public Func<T, Task<IEnumerable<T>>> GetChildrenFunc { get; set; } = null!;
    [CascadingParameter]
    public RenderFragment<T> ChildContent { get; set; } = null!;
    /// <summary>
    /// Depth starts at 0
    /// </summary>
    [CascadingParameter(Name="Depth")]
    public int Depth { get; set; }
    [CascadingParameter]
    public TreeViewWrapKey TreeViewWrapKey { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeView<T> TreeView { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<ITreeView[]> GetSiblingsAndSelfFunc { get; set; } = null!;
    [Parameter, EditorRequired]
    public int IndexAmongSiblings { get; set; }
    [Parameter, EditorRequired]
    public TreeView<T>? Parent { get; set; }

    private const int DEPTH_PADDING_LEFT_SCALING_IN_PIXELS = 12;

    /// <summary>
    /// This is used to ensure Children are only loaded once
    /// when the user expands and collapses many times in a row.
    ///
    /// A separate way to force refresh children needs to exist
    /// </summary>
    private bool _shouldLoadChildren = true;
    private ElementReference _titleSpan;
    private bool _isGettingChildren;
    private bool _previousFocusState;

    protected override void OnInitialized()
    {
        TreeViewWrapStateSelection.Select(x =>
        {
            x.Map.TryGetValue(TreeViewWrapKey, out var value);

            return value;
        });

        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        var localIsActive = IsActive;

        if (_previousFocusState != localIsActive)
        {
            _previousFocusState = localIsActive;

            _titleSpan.FocusAsync();
        }

        base.OnAfterRender(firstRender);
    }

    private bool IsActive => TreeViewWrapStateSelection.Value?.ActiveTreeViews
        .Select(x => x.Key)
        .Contains(TreeView.Key) ?? false;
    
    private string IsActiveStyling => IsActive
            ? "bstudio_active"
            : string.Empty;

    private string GetScaledByDepthPixelsOffset(int depth) => $"{depth * DEPTH_PADDING_LEFT_SCALING_IN_PIXELS}px";

    private async Task GetChildrenAsync()
    {
        TreeView.Children = (await GetChildrenFunc(TreeView.Item))
            .Select(x => (ITreeView) new TreeView<T>(TreeViewKey.NewTreeViewKey(), x)) 
            .ToArray();

        _shouldLoadChildren = false;
        _isGettingChildren = false;

        await InvokeAsync(StateHasChanged);
    }
    
    private void ToggleIsExpandedOnClick()
    {
        if (_shouldLoadChildren)
        {
            _ = Task.Run(async () =>
            {
                _isGettingChildren = true;
                await GetChildrenAsync();
                _isGettingChildren = false;
            });
        }

        TreeView.IsExpanded = !TreeView.IsExpanded;
    }
    
    private void SetIsActiveOnClick()
    {
        Dispatcher.Dispatch(new SetActiveTreeViewAction(TreeViewWrapKey, TreeView));

        _titleSpan.FocusAsync();
    }
    
    private void HandleOnKeyDown(KeyboardEventArgs keyboardEvent)
    {
        if (_previousFocusState == false)
            return;

        _previousFocusState = false;

        if (keyboardEvent.Key == KeyboardKeyFacts.MovementKeys.ARROW_RIGHT_KEY)
        {
            if (!TreeView.IsExpanded)
            {
                ToggleIsExpandedOnClick();
            }
            else
            {
                var children = TreeView.Children;

                if (children.Length > 0)
                {
                    Dispatcher.Dispatch(new SetActiveTreeViewAction(TreeViewWrapKey, children[0]));
                }
            }
        }
        else if (keyboardEvent.Key == KeyboardKeyFacts.MovementKeys.ARROW_LEFT_KEY)
        {
            if (TreeView.IsExpanded)
            {
                ToggleIsExpandedOnClick();
            }
            else
            {
                if (Parent is not null)
                {
                    Dispatcher.Dispatch(new SetActiveTreeViewAction(TreeViewWrapKey, Parent));
                }
            }
        }
        else
        {
            _previousFocusState = true;
        }
    }
}