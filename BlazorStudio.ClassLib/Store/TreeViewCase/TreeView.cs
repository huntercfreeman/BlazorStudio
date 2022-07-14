using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public class TreeView<T> : ITreeView
    where T : class
{
    public TreeView(TreeViewKey key, T item)
    {
        Key = key;
        Item = item;
    }

    public TreeViewKey Key { get; }
    public T Item { get; init; } = null!;
    public Type ItemType => typeof(T);
    public bool IsExpanded { get; set; }
    public ITreeView[] Children { get; set; } 
        = Array.Empty<ITreeView>();
}