namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public interface ITreeView
{
    public TreeViewKey Key { get; }
    public object? ItemUntyped { get; }
    public Type ItemType { get; }
    public bool IsExpanded { get; }
    public ITreeView[] Children { get; }
}