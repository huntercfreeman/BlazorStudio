using System.Collections.Immutable;
using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public interface ITreeViewWrap
{
    public TreeViewWrapKey Key { get; }
    public SequenceKey SequenceKey { get; set; }
    public Type ItemType { get; }
    public List<ITreeView> RootTreeViews { get; }
    public List<ITreeView> ActiveTreeViews { get; }
}