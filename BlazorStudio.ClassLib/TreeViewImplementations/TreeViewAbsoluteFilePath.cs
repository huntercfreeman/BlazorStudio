using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.TreeViewImplementations.Helper;
using BlazorTextEditor.RazorLib.TreeView;

namespace BlazorStudio.ClassLib.TreeViewImplementations;

public class TreeViewAbsoluteFilePath : TreeViewBase<IAbsoluteFilePath>
{
    public TreeViewAbsoluteFilePath(
        IAbsoluteFilePath absoluteFilePath,
        ICommonComponentRenderers commonComponentRenderers)
            : base(absoluteFilePath)
    {
        CommonComponentRenderers = commonComponentRenderers;
    }
 
    public ICommonComponentRenderers CommonComponentRenderers { get; }

    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj is not TreeViewNamespacePath treeViewSolutionExplorer ||
            treeViewSolutionExplorer.Item is null ||
            Item is null)
        {
            return false;
        }

        return treeViewSolutionExplorer.Item.AbsoluteFilePath
                   .GetAbsoluteFilePathString() == 
               Item.GetAbsoluteFilePathString();
    }

    public override int GetHashCode()
    {
        return Item?.GetAbsoluteFilePathString().GetHashCode() ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            CommonComponentRenderers.TreeViewAbsoluteFilePathRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(ITreeViewAbsoluteFilePathRendererType.TreeViewAbsoluteFilePath),
                    this
                },
            });
    }
    
    public override async Task LoadChildrenAsync()
    {
        if (Item is null)
            return;

        try
        {
            var newChildren = new List<TreeView>();
            
            if (Item.IsDirectory)
            {
                newChildren = await TreeViewHelper
                    .LoadChildrenForDirectoryAsync(this);
            }
        
            var oldChildrenMap = Children
                .ToDictionary(child => child);
        
            foreach (var newChild in newChildren)
            {
                if (oldChildrenMap.TryGetValue(newChild, out var oldChild))
                {
                    newChild.IsExpanded = oldChild.IsExpanded;
                    newChild.IsExpandable = oldChild.IsExpandable;
                    newChild.IsHidden = oldChild.IsHidden;
                    newChild.Id = oldChild.Id;
                    newChild.Children = oldChild.Children;
                }
            }
            
            for (int i = 0; i < newChildren.Count; i++)
            {
                var newChild = newChildren[i];
                
                newChild.IndexAmongSiblings = i;
                newChild.Parent = this;
            }
            
            Children = newChildren;
        }
        catch (Exception exception)
        {
            Children = new List<TreeView>
            {
                new TreeViewException(
                    exception,
                    CommonComponentRenderers)
                {
                    IsExpandable = false,
                    IsExpanded = false,
                    Parent = this,
                    IndexAmongSiblings = 0,
                }
            };
        }
    }

    public override void RemoveRelatedFilesFromParent(List<TreeView> treeViews)
    {
        // This method is meant to do nothing in this case.
    }
}