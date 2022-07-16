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
    public TreeViewDisplay<T>? Parent { get; set; }

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
                    Dispatcher.Dispatch(new SetActiveTreeViewAction(TreeViewWrapKey, Parent.TreeView));
                }
            }
        }
        else if (keyboardEvent.Key == KeyboardKeyFacts.MovementKeys.ARROW_UP_KEY)
        {
            if (IndexAmongSiblings == 0 &&
                Parent is not null)
            {
                Dispatcher.Dispatch(new SetActiveTreeViewAction(TreeViewWrapKey, Parent.TreeView));
            }
            else
            {
                var siblingsAndSelf = GetSiblingsAndSelfFunc.Invoke();

                if (IndexAmongSiblings > 0)
                {
                    RecursivelySetArrowUp(siblingsAndSelf[IndexAmongSiblings - 1]);
                }
            }
        }
        else if (keyboardEvent.Key == KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY)
        {
            var rememberTreeViewChildren = TreeView.Children;

            var siblingsAndSelf = GetSiblingsAndSelfFunc.Invoke();

            if (IndexAmongSiblings == siblingsAndSelf.Length - 1 && 
                Parent is not null)
            {
                var activeTreeViewChanged = RecursivelySetArrowDown(Parent);

                if (!activeTreeViewChanged)
                {
                    _previousFocusState = true;
                }
            }
            else if (TreeView.IsExpanded &&
                     rememberTreeViewChildren.Length > 0)
            {
                Dispatcher.Dispatch(new SetActiveTreeViewAction(TreeViewWrapKey, rememberTreeViewChildren[0]));
            }
            else
            {
                if (IndexAmongSiblings < siblingsAndSelf.Length - 1)
                {
                    Dispatcher.Dispatch(new SetActiveTreeViewAction(TreeViewWrapKey, siblingsAndSelf[IndexAmongSiblings + 1]));
                }
            }
        }
        else
        {
            _previousFocusState = true;
        }
    }

    private bool RecursivelySetArrowDown(TreeViewDisplay<T> treeViewDisplay)
    {
        var siblingsAndSelf = treeViewDisplay.GetSiblingsAndSelfFunc();

        if (treeViewDisplay.IndexAmongSiblings == siblingsAndSelf.Length - 1 &&
            treeViewDisplay.Parent is not null)
        {
            return RecursivelySetArrowDown(treeViewDisplay.Parent);
        }
        else if (treeViewDisplay.IndexAmongSiblings == siblingsAndSelf.Length - 1 &&
            treeViewDisplay.Parent is null)
        {
            return false;
        }
        else
        {
            Dispatcher.Dispatch(new SetActiveTreeViewAction(TreeViewWrapKey, siblingsAndSelf[treeViewDisplay.IndexAmongSiblings + 1]));
            return true;
        }
    }
    
    private void RecursivelySetArrowUp(ITreeView treeView)
    {
        if (treeView.IsExpanded &&
            treeView.Children.Length > 0)
        {
            RecursivelySetArrowUp(treeView.Children[^1]);
        }
        else
        {
            Dispatcher.Dispatch(new SetActiveTreeViewAction(TreeViewWrapKey, treeView));
        }
    }
}