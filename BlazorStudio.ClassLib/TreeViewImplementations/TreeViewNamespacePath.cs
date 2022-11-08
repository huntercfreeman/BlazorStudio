using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorStudio.ClassLib.TreeViewImplementations.Helper;
using BlazorTreeView.RazorLib;
using Fluxor;

namespace BlazorStudio.ClassLib.TreeViewImplementations;

public class TreeViewNamespacePath : TreeViewBase<NamespacePath>
{
    private readonly ITreeViewHelper _treeViewHelper;

    public TreeViewNamespacePath(
        NamespacePath namespacePath,
        ITreeViewHelper treeViewHelper)
            : base(namespacePath)
    {
        _treeViewHelper = treeViewHelper;
    }
    
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
               Item.AbsoluteFilePath.GetAbsoluteFilePathString();
    }

    public override int GetHashCode()
    {
        return Item?.AbsoluteFilePath.GetAbsoluteFilePathString().GetHashCode() ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _treeViewHelper.CommonComponentRenderers.TreeViewNamespacePathRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewNamespacePath),
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
            
            if (Item.AbsoluteFilePath.IsDirectory)
            {
                newChildren = await _treeViewHelper
                    .LoadChildrenForDirectoryAsync(Item);
            }
            else
            {
                switch (Item.AbsoluteFilePath.ExtensionNoPeriod)
                {
                    case ExtensionNoPeriodFacts.DOT_NET_SOLUTION:
                        newChildren = await _treeViewHelper
                            .LoadChildrenForDotNetSolutionAsync(Item);
                        break;
                    case ExtensionNoPeriodFacts.C_SHARP_PROJECT:
                        newChildren = await _treeViewHelper
                            .LoadChildrenForCSharpProjectAsync(Item);
                        break;
                }
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
                newChildren[i].IndexAmongSiblings = i;
            }
            
            Children = newChildren;
        }
        catch (Exception exception)
        {
            Children = new List<TreeView>
            {
                new TreeViewException(
                    exception,
                    _treeViewHelper)
                {
                    IsExpandable = false,
                    IsExpanded = false,
                    Parent = this,
                    IndexAmongSiblings = 0,
                }
            };
        }
    }

    
    
    
}