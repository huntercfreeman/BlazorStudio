using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public record TreeViewWrap<T> : ITreeViewWrap
    where T : class
{
    public TreeViewWrap(TreeViewWrapKey key)
    {
        Key = key;
    }

    public TreeViewWrapKey Key { get; }
    public Type ItemType => typeof(T);

    public ImmutableList<ITreeViewRecord> RootTreeViewRecords { get; init; } 
        = ImmutableList<ITreeViewRecord>.Empty;
}