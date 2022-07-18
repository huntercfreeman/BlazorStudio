using BlazorStudio.ClassLib.Errors;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PlainTextEditor.ClassLib.Keyboard;

namespace BlazorStudio.RazorLib.TreeViewCase;

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
    [CascadingParameter(Name="OnEnterKeyDown")]
    public Action<T> OnEnterKeyDown { get; set; } = null!;
    [CascadingParameter(Name= "OnSpaceKeyDown")]
    public Action<T> OnSpaceKeyDown { get; set; } = null!;
    /// <summary>
    /// If <see cref="OnContextMenu"/> is provided then:
    /// upon ContextMenuEvent event the Action will be invoked.
    /// 
    /// If ContextMenu event occurs with { 'F10' + 'ShiftKey' }
    /// the MouseEventArgs will be null.
    /// </summary>
    [CascadingParameter(Name="OnContextMenu")]
    public Action<T, MouseEventArgs?>? OnContextMenu { get; set; }
    /// <summary>
    /// If <see cref="OnContextMenuRenderFragment"/> is provided then:
    /// upon ContextMenuEvent event the RenderFragment will be rendered.
    ///
    /// If ContextMenu event occurs with { 'F10' + 'ShiftKey' }
    /// the MouseEventArgs will be null.
    /// </summary>
    [CascadingParameter(Name="OnContextMenuRenderFragment")]
    public RenderFragment<TreeViewWrapDisplay<T>.ContextMenuEventDto<T>>? OnContextMenuRenderFragment { get; set; }
    [CascadingParameter(Name = "IsExpandable")]
    public Func<T, bool> IsExpandable { get; set; } = null!;

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
    private RichErrorModel? _toggleIsExpandedOnClickRichErrorModel;
    private MouseEventArgs? _mostRecentMouseEventArgs;

    private Dimensions _fileDropdownDimensions = new()
    {
        DimensionsPositionKind = DimensionsPositionKind.Absolute,
        LeftCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = 0
            }
        },
        TopCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = 0
            }
        },
    };

    private DropdownKey _fileDropdownKey = DropdownKey.NewDropdownKey();

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

            if (_previousFocusState)
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

    private void GetChildrenAsync()
    {
        _ = TaskModelManagerService.EnqueueTaskModelAsync(async (cancellationToken) =>
            {
                _isGettingChildren = true;
                
                TreeView.Children = (await GetChildrenFunc(TreeView.Item))
                    .Select(x => (ITreeView)new TreeView<T>(TreeViewKey.NewTreeViewKey(), x))
                    .ToArray();

                _isGettingChildren = false;
                _shouldLoadChildren = false;
                
                await InvokeAsync(StateHasChanged);
            },
            $"{nameof(ToggleIsExpandedOnClick)}",
            false,
            TimeSpan.FromSeconds(10),
            HandleExceptionForToggleIsExpandedOnClick);
    }
    
    private void ToggleIsExpandedOnClick()
    {
        if (_shouldLoadChildren)
        {
            GetChildrenAsync();
        }

        TreeView.IsExpanded = !TreeView.IsExpanded;
    }
    
    private RichErrorModel HandleExceptionForToggleIsExpandedOnClick(Exception exception)
    {
        _isGettingChildren = false;
        _toggleIsExpandedOnClickRichErrorModel = new RichErrorModel(
            $"{nameof(ToggleIsExpandedOnClick)}: {exception.Message}",
            $"TODO: Add a hint");

        InvokeAsync(StateHasChanged);

        return _toggleIsExpandedOnClickRichErrorModel;
    }
    
    private void SetIsActiveOnClick()
    {
        Dispatcher.Dispatch(new SetActiveTreeViewAction(TreeViewWrapKey, TreeView));

        _titleSpan.FocusAsync();
    }

    /// <summary>
    /// Need to conditionally call PreventDefault
    ///
    /// Tab key to go to next focusable element should work
    ///
    /// However, ArrowDown to scroll the viewport should be prevent defaulted
    ///
    /// If the active tree view entry goes out of viewport then scroll manually
    /// </summary>
    /// <param name="keyboardEventArgs"></param>
    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (_previousFocusState == false)
            return;

        _previousFocusState = false;

        if (keyboardEventArgs.Key == KeyboardKeyFacts.MovementKeys.ARROW_RIGHT_KEY)
        {
            if (!IsExpandable(TreeView.Item))
                return;

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
        else if (keyboardEventArgs.Key == KeyboardKeyFacts.MovementKeys.ARROW_LEFT_KEY)
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
        else if (keyboardEventArgs.Key == KeyboardKeyFacts.MovementKeys.ARROW_UP_KEY)
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
        else if (keyboardEventArgs.Key == KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY)
        {
            var rememberTreeViewChildren = TreeView.Children;

            var siblingsAndSelf = GetSiblingsAndSelfFunc.Invoke();

            if (TreeView.IsExpanded &&
                rememberTreeViewChildren.Length > 0)
            {
                Dispatcher.Dispatch(new SetActiveTreeViewAction(TreeViewWrapKey, rememberTreeViewChildren[0]));
            }
            else if (IndexAmongSiblings == siblingsAndSelf.Length - 1 &&
                     Parent is not null)
            {
                var activeTreeViewChanged = RecursivelySetArrowDown(Parent);

                if (!activeTreeViewChanged)
                {
                    _previousFocusState = true;
                }
            }
            else
            {
                if (IndexAmongSiblings < siblingsAndSelf.Length - 1)
                {
                    Dispatcher.Dispatch(new SetActiveTreeViewAction(TreeViewWrapKey, siblingsAndSelf[IndexAmongSiblings + 1]));
                }
            }
        }
        else if (keyboardEventArgs.Key == KeyboardKeyFacts.WhitespaceKeys.ENTER_CODE)
        {
            OnEnterKeyDown(TreeView.Item);
        }
        else if (keyboardEventArgs.Key == KeyboardKeyFacts.WhitespaceKeys.SPACE_CODE)
        {
            OnSpaceKeyDown(TreeView.Item);
        }
        else if (KeyboardKeyFacts.CheckIsAlternateContextMenuEvent(keyboardEventArgs.Key, keyboardEventArgs.ShiftKey))
        {
            HandleOnContextMenu(null);
        }
        else
        {
            _previousFocusState = true;
        }
    }
    
    private void HandleOnContextMenu(MouseEventArgs? mouseEventArgs)
    {
        _mostRecentMouseEventArgs = mouseEventArgs;

        if (OnContextMenu is not null)
        {
            OnContextMenu(TreeView.Item, mouseEventArgs);
        }

        if (OnContextMenuRenderFragment is not null)
        {
            DispatchAddActiveDropdownKeyActionOnClick(_fileDropdownKey);
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

    private void DispatchAddActiveDropdownKeyActionOnClick(DropdownKey fileDropdownKey)
    {
        Dispatcher.Dispatch(new AddActiveDropdownKeyAction(fileDropdownKey));
    }
}