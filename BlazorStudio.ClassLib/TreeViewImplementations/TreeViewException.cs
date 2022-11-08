using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.TreeViewImplementations.Helper;
using BlazorTreeView.RazorLib;

namespace BlazorStudio.ClassLib.TreeViewImplementations;

public class TreeViewException : TreeViewBase<Exception>
{
    private readonly ITreeViewHelper _treeViewHelper;

    public TreeViewException(
        Exception? exception,
        ITreeViewHelper treeViewHelper)
            : base(exception)
    {
        _treeViewHelper = treeViewHelper;
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
        return new TreeViewRenderer(
            _treeViewHelper.CommonComponentRenderers.TreeViewExceptionRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(ITreeViewExceptionRendererType.TreeViewException),
                    this
                },
            });
    }
    
    public override Task LoadChildrenAsync()
    {
        return Task.CompletedTask;
    }
}