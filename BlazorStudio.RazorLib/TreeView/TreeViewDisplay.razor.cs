using BlazorStudio.ClassLib.CustomEvents;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.TreeView;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.TreeView;

public partial class TreeViewDisplay<TItem> : ComponentBase, IDisposable
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TreeViewModel<TItem> TreeViewModel { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment<TreeViewModel<TItem>> ItemRenderFragment { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment<TreeViewDisplayContextMenuEvent<TItem>> ContextMenuEventRenderFragment { get; set; } = null!;
    [Parameter]
    public bool ShouldShowRoot { get; set; } = true;
    [Parameter]
    public Func<TreeViewModel<TItem>>? GetRootFunc { get; set; }
    [Parameter]
    public int Depth { get; set; }
    [Parameter]
    public int Index { get; set; }
    [Parameter]
    public TreeViewDisplay<TItem>? ParentTreeViewDisplay { get; set; }

    private const int PADDING_LEFT_PER_DEPTH_IN_PIXELS = 25;
    
    private TreeViewModel<TItem>? _previousTreeViewModel;
    private ElementReference? _titleElementReference;
    private DropdownKey _contextMenuEventDropdownKey = DropdownKey.NewDropdownKey();
    private MouseEventArgs? _contextMenuCapturedMouseEventArgs;

    private TreeViewDisplayContextMenuEvent<TItem> ContextMenuEvent => new(
            TreeViewModel, 
            _contextMenuCapturedMouseEventArgs);

    private TreeViewModel<TItem> Root => GetRootFunc is null
        ? TreeViewModel
        : GetRootFunc.Invoke();

    private string IsActiveDescendantClassCss => IsActiveDescendant
        ? "bte_active"
        : string.Empty;
    
    private bool IsActiveDescendant =>
        Root.ActiveDescendant is not null && Root.ActiveDescendant.Id == TreeViewModel.Id;
    
    private bool IsRoot => Root.Id == TreeViewModel.Id;

    private int PaddingLeft => Depth * PADDING_LEFT_PER_DEPTH_IN_PIXELS;

    private string RootTabIndex => IsRoot 
        ? "0"
        : string.Empty;

    private int TitleTabIndex => IsActiveDescendant
        ? 0
        : -1;

    protected override Task OnParametersSetAsync()
    {
        if (_previousTreeViewModel is null ||
            _previousTreeViewModel.Id != TreeViewModel.Id)
        {
            if (_previousTreeViewModel is not null)
                _previousTreeViewModel.OnStateChanged -= TreeViewModelOnStateChanged;

            TreeViewModel.OnStateChanged += TreeViewModelOnStateChanged;
            _previousTreeViewModel = TreeViewModel;
        }
        
        return base.OnParametersSetAsync();
    }

    private void ToggleIsExpandedOnClick()
    {
        TreeViewModel.IsExpanded = !TreeViewModel.IsExpanded;
        FireAndForgetLoadChildren();
    }
    
    private void FireAndForgetLoadChildren()
    {
        _ = Task.Run(async () =>
        {
            await TreeViewModel.LoadChildrenFuncAsync.Invoke(TreeViewModel);
            await InvokeAsync(StateHasChanged);
        });
    }

    private void SetActiveDescendantOnClick()
    {
        SetActiveDescendantAndRerender(TreeViewModel);
    }
    
    private void SetActiveDescendantAndRerender(TreeViewModel<TItem> treeViewModel)
    {
        if (treeViewModel.Id == Root.Id &&
            !ShouldShowRoot)
        {
            // !ShouldShowRoot then don't let root get focused
            // Set focus instead to the root's first child if any

            if (Root.Children.Any())
            {
                SetActiveDescendantAndRerender(
                    Root.Children.First());
            }

            return;
        }
        
        var previousActiveDescendant = Root.ActiveDescendant; 
        
        Root.ActiveDescendant = treeViewModel;

        if (previousActiveDescendant is not null)
            previousActiveDescendant.InvokeOnStateChanged(false);
        
        Root.ActiveDescendant.InvokeOnStateChanged(true);
    }
    
    private async void TreeViewModelOnStateChanged(object? sender, bool setFocus)
    {
        await InvokeAsync(StateHasChanged);

        if (setFocus && _titleElementReference.HasValue)
            await _titleElementReference.Value.FocusAsync();
    }

    private void HandleRootOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN:
            {
                if (Root.ActiveDescendant is null)
                    SetActiveDescendantAndRerender(Root);
                else
                    SetActiveDescendantAndRerender(Root.ActiveDescendant);
                
                break;
            }
        }
    }

    private void HandleTitleOnCustomKeyDown(CustomKeyDownEventArgs customKeyDownEventArgs)
    {
        switch (customKeyDownEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_LEFT:
            {
                if (TreeViewModel.IsExpanded)
                    TreeViewModel.IsExpanded = false;
                else if (ParentTreeViewDisplay is not null)
                    SetActiveDescendantAndRerender(ParentTreeViewDisplay.TreeViewModel);
                
                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN:
            {
                if (TreeViewModel.IsExpanded && TreeViewModel.Children.Any())
                {
                    SetActiveDescendantAndRerender(TreeViewModel.Children.First());
                }
                else if (ParentTreeViewDisplay is not null)
                {
                    if (Index < ParentTreeViewDisplay.TreeViewModel.Children.Count - 1)
                    {
                        SetActiveDescendantAndRerender(
                            ParentTreeViewDisplay.TreeViewModel.Children[Index + 1]);
                    }
                    else
                    {
                        var targetDisplay = this;

                        while (targetDisplay.Index == (targetDisplay.ParentTreeViewDisplay?.TreeViewModel.Children.Count ?? 0) - 1)
                        {
                            if (targetDisplay.ParentTreeViewDisplay is null)
                                break;

                            targetDisplay = targetDisplay.ParentTreeViewDisplay;
                        }

                        if (targetDisplay.ParentTreeViewDisplay is null ||
                            targetDisplay.Index == (targetDisplay.ParentTreeViewDisplay?.TreeViewModel.Children.Count ?? 0) - 1)
                        {
                            break;
                        }

                        var model = targetDisplay.ParentTreeViewDisplay.TreeViewModel.Children[targetDisplay.Index + 1];

                        SetActiveDescendantAndRerender(model);
                    }
                }
                
                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_UP:
            {
                if (ParentTreeViewDisplay is null)
                    break;

                if (Index == 0)
                {
                    SetActiveDescendantAndRerender(ParentTreeViewDisplay.TreeViewModel);
                }
                else
                {
                    var model = ParentTreeViewDisplay.TreeViewModel.Children[Index - 1];

                    while (model.IsExpanded && model.Children.Any())
                    {
                        model = model.Children.Last();
                    }
                    
                    SetActiveDescendantAndRerender(model);
                }

                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_RIGHT:
            {
                if (TreeViewModel.IsExpanded && TreeViewModel.Children.Any())
                {
                    SetActiveDescendantAndRerender(TreeViewModel.Children.First());
                }
                else
                {
                    TreeViewModel.IsExpanded = true;
                    FireAndForgetLoadChildren();
                }

                break;
            }
            case KeyboardKeyFacts.MovementKeys.HOME:
            {
                if (customKeyDownEventArgs.CtrlWasPressed)
                {
                    if (ParentTreeViewDisplay is not null)
                    {
                        SetActiveDescendantAndRerender(ParentTreeViewDisplay.TreeViewModel.Children.First());
                    }
                }
                else
                {
                    // Later video add a while loop
                    SetActiveDescendantAndRerender(Root);
                }
                
                break;
            }
            case KeyboardKeyFacts.MovementKeys.END:
            {
                if (customKeyDownEventArgs.CtrlWasPressed)
                {
                    if (ParentTreeViewDisplay is not null)
                    {
                        SetActiveDescendantAndRerender(ParentTreeViewDisplay.TreeViewModel.Children.Last());
                    }
                }
                else if (Root.IsExpanded && Root.Children.Any())
                {
                    // Later video add a while loop
                    
                    SetActiveDescendantAndRerender(Root.Children.Last());
                }
                
                break;
            }
        }

        if (KeyboardKeyFacts.CheckIsContextMenuEvent(customKeyDownEventArgs))
            HandleTitleOnContextMenu(null);
    }
    
    private void HandleTitleOnContextMenu(MouseEventArgs? mouseEventArgs)
    {
        // mouseEventArgs is null -> came from HandleTitleOnCustomKeyDown
        // mouseEventArgs.Button != 2 -> non right click triggered the ContextMenuEvent
        if (mouseEventArgs is null ||
            mouseEventArgs.Button != 2)
        {
            _contextMenuCapturedMouseEventArgs = null;
        }
        else
        {
            _contextMenuCapturedMouseEventArgs = mouseEventArgs;
        }
        
        Dispatcher.Dispatch(new AddActiveDropdownKeyAction(_contextMenuEventDropdownKey));
    } 
    
    public void Dispose()
    {
        TreeViewModel.OnStateChanged -= TreeViewModelOnStateChanged;
    }
}