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
    /// <summary>
    /// If consuming the <see cref="TreeViewDisplay{TItem}"/>
    /// one should ignore this [Parameter]. I do not want to use
    /// [CascadingParameter] for these out of fear that
    /// something will cascade from a location I wasn't expecting
    /// and break things.
    /// </summary>
    [Parameter]
    public InternalParameters<TItem> InternalParameters { get; set; } = new();

    private const int PADDING_LEFT_PER_DEPTH_IN_PIXELS = 14;
    
    private TreeViewModel<TItem>? _previousTreeViewModel;
    private ElementReference? _titleElementReference;
    private DropdownKey _contextMenuEventDropdownKey = DropdownKey.NewDropdownKey();
    private MouseEventArgs? _contextMenuCapturedMouseEventArgs;

    private TreeViewDisplayContextMenuEvent<TItem> ContextMenuEvent => new(
            TreeViewModel, 
            _contextMenuCapturedMouseEventArgs);

    private TreeViewModel<TItem> Root => InternalParameters.GetRootFunc is null
        ? TreeViewModel
        : InternalParameters.GetRootFunc.Invoke();

    private string IsActiveDescendantClassCss => IsActiveDescendant
        ? "bstudio_active"
        : string.Empty;
    
    private bool IsActiveDescendant =>
        Root.ActiveDescendant is not null && Root.ActiveDescendant.Id == TreeViewModel.Id;
    
    private bool IsRoot => Root.Id == TreeViewModel.Id;

    private int PaddingLeft => InternalParameters.Depth * PADDING_LEFT_PER_DEPTH_IN_PIXELS;

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

    private InternalParameters<TItem> ConstructInternalParameters(int index)
    {
        int depth;

        if (IsRoot && !ShouldShowRoot)
        {
            depth = 0;
        }
        else
        {
            depth = InternalParameters.Depth + 1;
        }
        
        return new InternalParameters<TItem>
        {
            Index = index,
            Depth = depth,
            GetRootFunc = () => Root,
            ParentTreeViewDisplay = this
        };
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
                else if (InternalParameters.ParentTreeViewDisplay is not null)
                    SetActiveDescendantAndRerender(InternalParameters.ParentTreeViewDisplay.TreeViewModel);
                
                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN:
            {
                if (TreeViewModel.IsExpanded && TreeViewModel.Children.Any())
                {
                    SetActiveDescendantAndRerender(TreeViewModel.Children.First());
                }
                else if (InternalParameters.ParentTreeViewDisplay is not null)
                {
                    if (InternalParameters.Index < InternalParameters.ParentTreeViewDisplay.TreeViewModel.Children.Count - 1)
                    {
                        SetActiveDescendantAndRerender(
                            InternalParameters.ParentTreeViewDisplay.TreeViewModel.Children[InternalParameters.Index + 1]);
                    }
                    else
                    {
                        var targetDisplay = this;

                        while (targetDisplay.InternalParameters.Index == (targetDisplay.InternalParameters.ParentTreeViewDisplay?.TreeViewModel.Children.Count ?? 0) - 1)
                        {
                            if (targetDisplay.InternalParameters.ParentTreeViewDisplay is null)
                                break;

                            targetDisplay = targetDisplay.InternalParameters.ParentTreeViewDisplay;
                        }

                        if (targetDisplay.InternalParameters.ParentTreeViewDisplay is null ||
                            targetDisplay.InternalParameters.Index == (targetDisplay.InternalParameters.ParentTreeViewDisplay?.TreeViewModel.Children.Count ?? 0) - 1)
                        {
                            break;
                        }

                        var model = targetDisplay.InternalParameters.ParentTreeViewDisplay.TreeViewModel.Children
                            [targetDisplay.InternalParameters.Index + 1];

                        SetActiveDescendantAndRerender(model);
                    }
                }
                
                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_UP:
            {
                if (InternalParameters.ParentTreeViewDisplay is null)
                    break;

                if (InternalParameters.Index == 0)
                {
                    SetActiveDescendantAndRerender(InternalParameters.ParentTreeViewDisplay.TreeViewModel);
                }
                else
                {
                    var model = InternalParameters.ParentTreeViewDisplay.TreeViewModel.Children
                        [InternalParameters.Index - 1];

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
                    if (InternalParameters.ParentTreeViewDisplay is not null)
                    {
                        SetActiveDescendantAndRerender(
                            InternalParameters.ParentTreeViewDisplay.TreeViewModel.Children
                                .First());
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
                    if (InternalParameters.ParentTreeViewDisplay is not null)
                    {
                        SetActiveDescendantAndRerender(
                            InternalParameters.ParentTreeViewDisplay.TreeViewModel.Children
                                .Last());
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