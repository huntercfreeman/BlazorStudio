using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public class TreeViewWrap<T> : ITreeViewWrap
    where T : class
{
    public TreeViewWrap(TreeViewWrapKey key)
    {
        Key = key;
    }

    public TreeViewWrapKey Key { get; }
    public Type ItemType => typeof(T);

    public ITreeView[] RootTreeViews { get; init; } 
        = Array.Empty<ITreeView>();
}