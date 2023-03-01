using BlazorCommon.RazorLib.TreeView.TreeViewClasses;

namespace BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;

public class TreeViewException : TreeViewWithType<Exception>
{
    private readonly ITreeViewRenderers _treeViewRenderers;

    public TreeViewException(
        Exception exception,
        bool isExpandable,
        bool isExpanded,
        ITreeViewRenderers treeViewRenderers)
        : base(
            exception,
            isExpandable,
            isExpanded)
    {
        _treeViewRenderers = treeViewRenderers;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj is not TreeViewException treeViewException)
        {
            return false;
        }

        return treeViewException.Item == Item;
    }

    public override int GetHashCode()
    {
        return Item?.GetHashCode() ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _treeViewRenderers.TreeViewExceptionRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewException),
                    this
                },
            });
    }

    public override Task LoadChildrenAsync()
    {
        return Task.CompletedTask;
    }
}