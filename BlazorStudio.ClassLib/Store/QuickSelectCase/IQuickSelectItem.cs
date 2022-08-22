namespace BlazorStudio.ClassLib.Store.QuickSelectCase;

public interface IQuickSelectItem
{
    public string DisplayName { get; }
    public object ItemNoType { get; }
    public Type ItemType { get; }
}