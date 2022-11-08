using BlazorTreeView.RazorLib;

namespace BlazorStudio.ClassLib.TreeViewImplementations;

public class TreeViewException : TreeViewBase<Exception>
{
    private readonly TreeViewRenderer _treeViewRenderer;

    public TreeViewException(
        Exception? exception,
        TreeViewRenderer treeViewRenderer)
            : base(exception)
    {
        _treeViewRenderer = treeViewRenderer;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj is not TreeViewException treeViewException ||
            treeViewException.Item is null ||
            Item is null)
        {
            return false;
        }

        return treeViewException.Item.Message == Item.Message;
    }

    public override int GetHashCode()
    {
        return Item?.Message.GetHashCode() ?? default;
    }
    
    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return _treeViewRenderer with
        {
            DynamicComponentParameters = new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewException),
                    this
                },
            }
        };
    }
    
    public override Task LoadChildrenAsync()
    {
        return Task.CompletedTask;
    }
}