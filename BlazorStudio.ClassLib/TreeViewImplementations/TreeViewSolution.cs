using BlazorCommon.RazorLib.TreeView.TreeViewClasses;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.DotNet;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.TreeViewImplementations.Helper;
using Fluxor;

namespace BlazorStudio.ClassLib.TreeViewImplementations;

public class TreeViewSolution : TreeViewWithType<DotNetSolution>
{
    public TreeViewSolution(
        DotNetSolution dotNetSolution,
        ICommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        bool isExpandable,
        bool isExpanded)
            : base(
                dotNetSolution,
                isExpandable,
                isExpanded)
    {
        CommonComponentRenderers = commonComponentRenderers;
        FileSystemProvider = fileSystemProvider;
        EnvironmentProvider = environmentProvider;
    }
 
    public ICommonComponentRenderers CommonComponentRenderers { get; }
    public IFileSystemProvider FileSystemProvider { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }

    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj is not TreeViewSolution treeViewSolution ||
            treeViewSolution.Item is null ||
            Item is null)
        {
            return false;
        }

        return treeViewSolution.Item.NamespacePath.AbsoluteFilePath.GetAbsoluteFilePathString() ==
               Item.NamespacePath.AbsoluteFilePath.GetAbsoluteFilePathString();
    }

    public override int GetHashCode()
    {
        return Item?.NamespacePath.AbsoluteFilePath
            .GetAbsoluteFilePathString()
            .GetHashCode()
               ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            CommonComponentRenderers.TreeViewNamespacePathRendererType!,
            new Dictionary<string, object?>
            {
                {
                    nameof(ITreeViewNamespacePathRendererType.NamespacePath),
                    Item?.NamespacePath
                },
            });
    }
    
    public override async Task LoadChildrenAsync()
    {
        if (Item is null)
            return;

        try
        {
            var newChildren = await TreeViewHelper
                .LoadChildrenForDotNetSolutionAsync(this);
            
            var oldChildrenMap = Children
                .ToDictionary(child => child);
        
            foreach (var newChild in newChildren)
            {
                if (oldChildrenMap.TryGetValue(newChild, out var oldChild))
                {
                    newChild.IsExpanded = oldChild.IsExpanded;
                    newChild.IsExpandable = oldChild.IsExpandable;
                    newChild.IsHidden = oldChild.IsHidden;
                    newChild.TreeViewNodeKey = oldChild.TreeViewNodeKey;
                    newChild.Children = oldChild.Children;
                }
            }
            
            for (int i = 0; i < newChildren.Count; i++)
            {
                var newChild = newChildren[i];
                
                newChild.IndexAmongSiblings = i;
                newChild.Parent = this;
                newChild.TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();
            }
            
            Children = newChildren;
        }
        catch (Exception exception)
        {
            Children = new List<TreeViewNoType>
            {
                new TreeViewException(
                    exception,
                    CommonComponentRenderers,
                    false,
                    false)
                {
                    Parent = this,
                    IndexAmongSiblings = 0,
                }
            };
        }
        
        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        return;
    }
}