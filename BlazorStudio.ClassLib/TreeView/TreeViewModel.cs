using BlazorStudio.ClassLib.FileSystem.Interfaces;

namespace BlazorStudio.ClassLib.TreeView;

public class TreeViewModel<TItem> : ITreeViewModel
{
    private bool _isExpanded;

    public TreeViewModel(
        TItem item, 
        bool canToggleExpandable,
        Func<TreeViewModel<TItem>, Task> loadChildrenFuncAsync)
    {
        Item = item;
        CanToggleExpandable = canToggleExpandable;
        LoadChildrenFuncAsync = loadChildrenFuncAsync;
    }
    
    public Type ItemType => typeof(TItem);
    public object UntypedItem => Item;
    public TItem Item { get; set; }
    public Func<TreeViewModel<TItem>, Task> LoadChildrenFuncAsync { get; set; }
    /// <summary>
    /// <see cref="Children"/> is being used for example: when filesystem directory
    /// contains filesystem entries. These are being considered children.
    /// </summary>
    public List<TreeViewModel<TItem>> Children { get; } = new();
    /// <summary>
    /// <see cref="NestedSiblings"/> is being used for when
    /// a child is marked as 'hidden' from the list of its parent
    /// and instead nested behind a sibling. 'hidden' marking
    /// does not remove the child from the parent's list. It
    /// is a boolean that is used for rendering the user interface.
    /// <br/><br/>
    /// An example of a nested sibling is when a directory contains
    /// a file named, 'Component.razor' and 'Component.razor.cs'.
    /// In this situation there is a codebehind file for a razor markup file.
    /// Therefore the razor markup takes from the parent directory its sibling file,
    /// the codebehind, and nests the codebehind behind itself.
    /// </summary>
    public List<TreeViewModel<TItem>> NestedSiblings { get; } = new();
    public Guid Id { get; } = Guid.NewGuid();
    public bool IsDisplayed { get; set; } = true;

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (!CanToggleExpandable)
                return;
                
            _isExpanded = value;
        }
    }

    public bool CanToggleExpandable { get; set; }
    /// <summary>
    /// This is used when dealing with codebehinds
    /// </summary>
    public bool ParentIsSibling { get; set; }
    public bool IsDeleted { get; set; }
    public TreeViewModel<TItem>? ActiveDescendant { get; set; }

    public event EventHandler<bool>? OnStateChanged;
    public event EventHandler? OnActiveDescendantChanged;

    public void InvokeOnStateChanged(bool setFocus)
    {
        OnStateChanged?.Invoke(this, setFocus);
    }

    public void RestoreState(TreeViewModel<TItem> previousTreeViewModel)
    {
        IsExpanded = previousTreeViewModel.IsExpanded;
        Children.Clear();
        Children.AddRange(previousTreeViewModel.Children);
        IsDisplayed = previousTreeViewModel.IsDisplayed;
        ParentIsSibling = false;
    }
}