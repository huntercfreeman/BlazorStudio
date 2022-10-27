namespace BlazorStudio.ClassLib.TreeView;

public class TreeViewModel<TItem> : ITreeViewModel
{
    public TreeViewModel(
        TItem item, 
        Func<TreeViewModel<TItem>, Task> loadChildrenFuncAsync)
    {
        Item = item;
        LoadChildrenFuncAsync = loadChildrenFuncAsync;
    }
    
    public TreeViewModel(
        TItem item, 
        Func<TreeViewModel<TItem>, Task> loadChildrenFuncAsync,
        TreeViewKey treeViewKey)
            : this(
                item, 
                loadChildrenFuncAsync)
    {
        TreeViewKey = treeViewKey;
    }

    public TreeViewKey TreeViewKey { get; } = TreeViewKey.NewTreeViewKey();
    public Type ItemType => typeof(TItem);
    public object UntypedItem => Item;
    public TItem Item { get; set; }
    public Func<TreeViewModel<TItem>, Task> LoadChildrenFuncAsync { get; set; }
    public List<TreeViewModel<TItem>> Children { get; } = new();
    public Guid Id { get; } = Guid.NewGuid();
    public bool IsDisplayed { get; set; }
    public bool IsExpanded { get; set; }
    public TreeViewModel<TItem>? ActiveDescendant { get; set; }

    public event EventHandler<bool>? OnStateChanged;
    public event EventHandler? OnActiveDescendantChanged;

    public void InvokeOnStateChanged(bool setFocus)
    {
        OnStateChanged?.Invoke(this, setFocus);
    }
}