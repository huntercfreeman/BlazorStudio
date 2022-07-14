using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public interface ITreeViewWrap
{
    public TreeViewWrapKey Key { get; }
    public Type ItemType { get; }
    public ITreeView[] RootTreeViews { get; }
}