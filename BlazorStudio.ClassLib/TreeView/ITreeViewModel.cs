namespace BlazorStudio.ClassLib.TreeView;

public interface ITreeViewModel
{
    public Type ItemType { get; }
    public object UntypedItem { get; }
}