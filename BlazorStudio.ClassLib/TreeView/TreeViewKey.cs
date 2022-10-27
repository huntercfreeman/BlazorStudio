namespace BlazorStudio.ClassLib.TreeView;

public record TreeViewKey(Guid Guid)
{
    public static TreeViewKey NewTreeViewKey()
    {
        return new TreeViewKey(Guid.NewGuid());
    }
}