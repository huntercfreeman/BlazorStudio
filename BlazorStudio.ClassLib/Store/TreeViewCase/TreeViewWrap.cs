using System.Collections.Immutable;
using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public class TreeViewWrap<T> : ITreeViewWrap
    where T : class
{
    public TreeViewWrap(TreeViewWrapKey key)
    {
        Key = key;
        SequenceKey = SequenceKey.NewSequenceKey();
    }

    public TreeViewWrapKey Key { get; }
    public SequenceKey SequenceKey { get; set; }
    public Type ItemType => typeof(T);

    public List<ITreeView> RootTreeViews { get; init; } = new();
    public List<ITreeView> ActiveTreeViews { get; } = new();
}