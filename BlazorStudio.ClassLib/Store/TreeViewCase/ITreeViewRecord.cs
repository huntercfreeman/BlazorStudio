using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public interface ITreeViewRecord
{
    public TreeViewKey Key { get; }
    public Type ItemType { get; }
    public bool IsExpanded { get; }
    public ImmutableList<ITreeViewRecord> Children { get; }
}