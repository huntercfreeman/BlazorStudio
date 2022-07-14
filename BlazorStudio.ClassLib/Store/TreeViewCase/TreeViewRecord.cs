using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public record TreeViewRecord<T> : ITreeViewRecord
    where T : class
{
    public TreeViewRecord(TreeViewKey key)
    {
        Key = key;
    }

    public TreeViewKey Key { get; }
    public T Item { get; init; } = null!;
    public Type ItemType => typeof(T);
    public bool IsExpanded { get; init; }
    public ImmutableList<ITreeViewRecord> Children { get; init; } 
        = ImmutableList<ITreeViewRecord>.Empty;
}