using BlazorStudio.ClassLib.Errors;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.ClassLib.Store.TreeViewCase;
using BlazorStudio.ClassLib.TaskModelManager;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.RazorLib.CustomEvents;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.TreeViewCase;

public partial class TreeViewDisplay<T>
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
    public Action<TreeViewKeyboardEventDto<T>> OnEnterKeyDown { get; set; } = null!;
    [CascadingParameter(Name= "OnSpaceKeyDown")]
    public Action<TreeViewKeyboardEventDto<T>> OnSpaceKeyDown { get; set; } = null!;
    [CascadingParameter(Name= "OnDoubleClick")]
    public Action<TreeViewMouseEventDto<T>>? OnDoubleClick { get; set; } = null!;
    /// <summary>
    /// If <see cref="OnContextMenu"/> is provided then:
    /// upon ContextMenuEvent event the Action will be invoked.
    /// 
    /// If ContextMenu event occurs with { 'F10' + 'ShiftKey' }
    /// the MouseEventArgs will be null.
    /// </summary>
    [CascadingParameter(Name="OnContextMenu")]
    public Action<TreeViewContextMenuEventDto<T>>? OnContextMenu { get; set; }
    /// <summary>
    /// If <see cref="OnContextMenuRenderFragment"/> is provided then:
    /// upon ContextMenuEvent event the RenderFragment will be rendered.
    ///
    /// If ContextMenu event occurs with { 'F10' + 'ShiftKey' }
    /// the MouseEventArgs will be null.
    /// </summary>
    [CascadingParameter(Name="OnContextMenuRenderFragment")]
    public RenderFragment<TreeViewContextMenuEventDto<T>>? OnContextMenuRenderFragment { get; set; }
    [CascadingParameter(Name = "IsExpandable")]
    public Func<T, bool> IsExpandable { get; set; } = null!;
    [CascadingParameter(Name = "ReloadRoot")]
    public Func<Task> ReloadRoot { get; set; } = null!;

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
                Value = 5
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

        InvokeAsync(StateHasChanged);
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
    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (_previousFocusState == false)
            return;

        _previousFocusState = false;

        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT_KEY:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_RIGHT_KEY:
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

                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT_KEY:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_LEFT_KEY:
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

                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_UP_KEY:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_UP_KEY:
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

                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN_KEY:
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

                break;
            }
            case KeyboardKeyFacts.WhitespaceKeys.ENTER_CODE:
            {
                OnEnterKeyDown(new TreeViewKeyboardEventDto<T>(keyboardEventArgs, 
                    TreeView.Item, 
                    ToggleIsExpandedOnClick, 
                    DispatchSetSelfAsActiveTreeView,
                    RefreshTreeViewTargetAsync, 
                    RefreshParentOfTreeViewTargetAsync));

                break;
            }
            case KeyboardKeyFacts.WhitespaceKeys.SPACE_CODE:
            {
                OnSpaceKeyDown(new TreeViewKeyboardEventDto<T>(keyboardEventArgs, 
                    TreeView.Item, 
                    ToggleIsExpandedOnClick, 
                    DispatchSetSelfAsActiveTreeView,
                    RefreshTreeViewTargetAsync, 
                    RefreshParentOfTreeViewTargetAsync));

                break;
            }
            default:
            {
                if (KeyboardKeyFacts.CheckIsContextMenuEvent(keyboardEventArgs.Key, keyboardEventArgs.ShiftKey))
                {
                    HandleOnContextMenu(null);
                }
                else
                {
                    _previousFocusState = true;
                }

                break;
            }
        }
    }
    
    private void HandleOnContextMenu(MouseEventArgs? mouseEventArgs)
    {
        _mostRecentMouseEventArgs = mouseEventArgs;

        if (OnContextMenu is not null)
        {
            OnContextMenu(new TreeViewContextMenuEventDto<T>(mouseEventArgs, 
                null, 
                TreeView.Item, 
                ToggleIsExpandedOnClick, 
                DispatchSetSelfAsActiveTreeView,
                RefreshTreeViewTargetAsync, 
                RefreshParentOfTreeViewTargetAsync,
                _titleSpan));
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
    
    private void HandleOnDoubleClick(MouseEventArgs mouseEventArgs)
    {
        if (OnDoubleClick is not null)
        {
            OnDoubleClick.Invoke(
                new TreeViewMouseEventDto<T>(mouseEventArgs, 
                    TreeView.Item, 
                    ToggleIsExpandedOnClick, 
                    DispatchSetSelfAsActiveTreeView,
                    RefreshTreeViewTargetAsync, 
                    RefreshParentOfTreeViewTargetAsync));
        }
    }

    private void DispatchSetSelfAsActiveTreeView()
    {
        Dispatcher.Dispatch(new SetActiveTreeViewAction(TreeViewWrapKey, TreeView));
    }
    
    private Task RefreshTreeViewTargetAsync()
    {
        GetChildrenAsync();
        return Task.CompletedTask;
    }
    
    private async Task RefreshParentOfTreeViewTargetAsync()
    {
        if (Parent is not null)
        {
            Parent.GetChildrenAsync();
        }
        else
        {
            await ReloadRoot.Invoke();
        }
    }

    private void DispatchAddActiveDropdownKeyActionOnClick(DropdownKey fileDropdownKey)
    {
        Dispatcher.Dispatch(new AddActiveDropdownKeyAction(fileDropdownKey));
    }
    
    private void HandleCustomOnKeyDown(CustomKeyDown customKeyDown)
    {
        var z = 2;
    }
}