namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public record TreeViewKey(Guid Guid)
{
    public static TreeViewKey NewTreeViewKey()
    {
        return new TreeViewKey(Guid.NewGuid());
    }
}