using BlazorCommon.RazorLib.TreeView.TreeViewClasses;

namespace BlazorStudio.RazorLib.BalcTreeView.TreeViewImplementations;

public class TreeViewText : TreeViewWithType<string>
{
    private readonly ITreeViewRenderers _treeViewRenderers;

    public TreeViewText(
        string text,
        bool isExpandable,
        bool isExpanded,
        ITreeViewRenderers treeViewRenderers)
        : base(
            text,
            isExpandable,
            isExpanded)
    {
        _treeViewRenderers = treeViewRenderers;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj is not TreeViewText treeViewText)
        {
            return false;
        }

        return treeViewText.Item == Item;
    }

    public override int GetHashCode()
    {
        return Item?.GetHashCode() ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _treeViewRenderers.TreeViewTextRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewText),
                    this
                },
            });
    }

    public override Task LoadChildrenAsync()
    {
        return Task.CompletedTask;
    }
}