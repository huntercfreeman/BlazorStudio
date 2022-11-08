using BlazorStudio.ClassLib.Namespaces;
using BlazorTreeView.RazorLib;

namespace BlazorStudio.ClassLib.TreeViewImplementations;

public class TreeViewSolutionExplorer : TreeViewBase<NamespacePath>
{
    private readonly TreeViewRenderer _treeViewRenderer;

    public TreeViewSolutionExplorer(
        NamespacePath namespacePath, 
        TreeViewRenderer treeViewRenderer)
            : base(namespacePath)
    {
        _treeViewRenderer = treeViewRenderer;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj is not TreeViewSolutionExplorer treeViewSolutionExplorer ||
            treeViewSolutionExplorer.Item is null ||
            Item is null)
        {
            return false;
        }

        return treeViewSolutionExplorer.Item.AbsoluteFilePath
                   .GetAbsoluteFilePathString() ==
               Item.AbsoluteFilePath.GetAbsoluteFilePathString();
    }

    public override int GetHashCode()
    {
        return Item?.AbsoluteFilePath.GetAbsoluteFilePathString().GetHashCode() ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return _treeViewRenderer with {
            DynamicComponentParameters = new Dictionary<string, object?>
            {
                {
                    "TreeViewSolutionExplorer",
                    this
                },
            } 
        };
    }
    
    public override Task LoadChildrenAsync()
    {
        if (Item is null)
            return Task.CompletedTask;

        // try
        // {
        //     var indexAmongSiblings = 0;
        //     
        //     var childDirectories = Directory
        //         .GetDirectories(Item.FullName)
        //         .Select(dirPath => new DirectoryInfo(dirPath))
        //         .Select(dirInfo => (TreeView)new TreeViewDirectoryInfo(dirInfo)
        //         {
        //             IsExpandable = true,
        //             IsExpanded = false,
        //             Parent = this,
        //             IndexAmongSiblings = indexAmongSiblings++,
        //         });
        //
        //     var childFiles = Directory
        //         .GetFiles(Item.FullName)
        //         .Select(filePath => new FileInfo(filePath))
        //         .Select(fileInfo => (TreeView)new TreeViewFileInfo(fileInfo)
        //         {
        //             IsExpandable = false,
        //             IsExpanded = false,
        //             Parent = this,
        //             IndexAmongSiblings = indexAmongSiblings++,
        //         });
        //
        //     var newChildren = childDirectories
        //         .Union(childFiles)
        //         .ToList();
        //
        //     var oldChildrenMap = Children
        //         .ToDictionary(child => child);
        //
        //     foreach (var newChild in newChildren)
        //     {
        //         if (oldChildrenMap.TryGetValue(newChild, out var oldChild))
        //         {
        //             newChild.IsExpanded = oldChild.IsExpanded;
        //             newChild.IsExpandable = oldChild.IsExpandable;
        //             newChild.IsHidden = oldChild.IsHidden;
        //             newChild.Id = oldChild.Id;
        //             newChild.Children = oldChild.Children;
        //         }
        //     }
        //     
        //     Children = newChildren;
        // }
        // catch (Exception exception)
        // {
        //     Children = new List<TreeView>
        //     {
        //         new TreeViewException(exception)
        //         {
        //             IsExpandable = true,
        //             IsExpanded = false,
        //             Parent = this,
        //             IndexAmongSiblings = 0,
        //         }
        //     };
        // }
        
        return Task.CompletedTask;
    }
}