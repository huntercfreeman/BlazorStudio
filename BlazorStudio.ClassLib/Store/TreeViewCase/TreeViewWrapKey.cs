namespace BlazorStudio.ClassLib.Store.TreeViewCase;

public record TreeViewWrapKey(Guid Guid)
{
    public static TreeViewWrapKey NewTreeViewWrapKey()
    {
        return new TreeViewWrapKey(Guid.NewGuid());
    }
}