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
    [Parameter]
    public RenderFragment<TreeViewModel<TItem>> ContextMenuEventRenderFragment { get; set; } = null!;
    [Parameter]
    public Func<TreeViewModel<TItem>, CustomKeyDownEventArgs, Task>? OnAfterKeyDown { get; set; }
    [Parameter]
    public TreeViewDisplayInternalParameters<TItem>? InternalUseOnly { get; set; }

    private const int PADDING_LEFT_PER_DEPTH_IN_PIXELS = 25;
    
    private TreeViewModel<TItem>? _previousTreeViewModel;
    private ElementReference? _titleElementReference;
    private DropdownKey _contextMenuEventDropdownKey = DropdownKey.NewDropdownKey();

    private TreeViewModel<TItem> Root => InternalUseOnly?.GetRootFunc is null
        ? TreeViewModel
        : InternalUseOnly.GetRootFunc.Invoke();

    private string IsActiveDescendantClassCss => IsActiveDescendant
        ? "bte_active"
        : string.Empty;
    
    private bool IsActiveDescendant =>
        Root.ActiveDescendant is not null && Root.ActiveDescendant.Id == TreeViewModel.Id;

    private int PaddingLeft => (InternalUseOnly?.Depth ?? 0) * PADDING_LEFT_PER_DEPTH_IN_PIXELS;

    private string RootTabIndex => Root.Id == TreeViewModel.Id
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
                else if (InternalUseOnly?.ParentTreeViewDisplay is not null)
                    SetActiveDescendantAndRerender(InternalUseOnly.ParentTreeViewDisplay.TreeViewModel);
                
                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN:
            {
                if (TreeViewModel.IsExpanded && TreeViewModel.Children.Any())
                {
                    SetActiveDescendantAndRerender(TreeViewModel.Children.First());
                }
                else if (InternalUseOnly?.ParentTreeViewDisplay is not null)
                {
                    if (InternalUseOnly.Index < InternalUseOnly.ParentTreeViewDisplay.TreeViewModel.Children.Count - 1)
                    {
                        SetActiveDescendantAndRerender(
                            InternalUseOnly.ParentTreeViewDisplay.TreeViewModel.Children[InternalUseOnly.Index + 1]);
                    }
                    else
                    {
                        var targetDisplay = this;

                        while (targetDisplay.InternalUseOnly.Index == (targetDisplay.InternalUseOnly.ParentTreeViewDisplay?.TreeViewModel.Children.Count ?? 0) - 1)
                        {
                            if (targetDisplay.InternalUseOnly.ParentTreeViewDisplay is null)
                                break;

                            targetDisplay = targetDisplay.InternalUseOnly.ParentTreeViewDisplay;
                        }

                        if (targetDisplay.InternalUseOnly.ParentTreeViewDisplay is null ||
                            targetDisplay.InternalUseOnly.Index == (targetDisplay.InternalUseOnly.ParentTreeViewDisplay?.TreeViewModel.Children.Count ?? 0) - 1)
                        {
                            break;
                        }

                        var model = targetDisplay.InternalUseOnly.ParentTreeViewDisplay.TreeViewModel.Children[targetDisplay.InternalUseOnly.Index + 1];

                        SetActiveDescendantAndRerender(model);
                    }
                }
                
                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_UP:
            {
                if (InternalUseOnly?.ParentTreeViewDisplay is null)
                    break;

                if (InternalUseOnly.Index == 0)
                {
                    SetActiveDescendantAndRerender(InternalUseOnly.ParentTreeViewDisplay.TreeViewModel);
                }
                else
                {
                    var model = InternalUseOnly.ParentTreeViewDisplay.TreeViewModel.Children[InternalUseOnly.Index - 1];

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
                    if (InternalUseOnly?.ParentTreeViewDisplay is not null)
                    {
                        SetActiveDescendantAndRerender(InternalUseOnly.ParentTreeViewDisplay.TreeViewModel.Children.First());
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
                    if (InternalUseOnly?.ParentTreeViewDisplay is not null)
                    {
                        SetActiveDescendantAndRerender(InternalUseOnly.ParentTreeViewDisplay.TreeViewModel.Children.Last());
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

        if (KeyboardKeyFacts.CheckIsContextMenuEvent(
                customKeyDownEventArgs.Key,
                customKeyDownEventArgs.Code,
                customKeyDownEventArgs.ShiftWasPressed,
                customKeyDownEventArgs.AltWasPressed))
        {
            Dispatcher.Dispatch(new AddActiveDropdownKeyAction(_contextMenuEventDropdownKey));
        }

        OnAfterKeyDown?.Invoke(TreeViewModel, customKeyDownEventArgs);
    }
    
    public void Dispose()
    {
        TreeViewModel.OnStateChanged -= TreeViewModelOnStateChanged;
    }
}