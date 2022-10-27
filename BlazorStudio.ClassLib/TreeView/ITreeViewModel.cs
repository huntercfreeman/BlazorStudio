namespace BlazorStudio.ClassLib.TreeView;

public interface ITreeViewModel
{
    public TreeViewKey TreeViewKey { get; }
    public Type ItemType { get; }
    public object UntypedItem { get; }
}