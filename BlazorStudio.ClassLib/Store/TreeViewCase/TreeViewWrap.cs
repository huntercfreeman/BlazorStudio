using BlazorStudio.ClassLib.Sequence;

namespace BlazorStudio.ClassLib.Store.TreeViewCase;

/// <summary>
/// This class is written poorly.
///
/// I need a new instance I suppose to fire Fluxor re-render of Blazor Component.
///
/// This needs to be rewritten immutably.
/// </summary>
/// <typeparam name="T"></typeparam>
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
    public List<ITreeView> ActiveTreeViews { get; init; } = new();

    public ITreeViewWrap CloneShallow()
    {
        return new TreeViewWrap<T>(Key)
        {
            RootTreeViews = RootTreeViews,
            ActiveTreeViews = ActiveTreeViews,
            SequenceKey = SequenceKey.NewSequenceKey()
        };
    }
}