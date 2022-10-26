namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public class TreeView<T> : ITreeView
{
    public TreeView(TreeViewKey key, T item)
    {
        Key = key;
        Item = item;
    }

    public T Item { get; init; }

    public TreeViewKey Key { get; }
    public object ItemUntyped => Item;
    public Type ItemType => typeof(T);
    public bool IsExpanded { get; set; }
    public ITreeView[] Children { get; set; }
        = Array.Empty<ITreeView>();
}