using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.BaseTypes;
using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorStudio.ClassLib.TreeViewImplementations.Helper;
using Fluxor;

namespace BlazorStudio.ClassLib.TreeViewImplementations;

public class TreeViewNamespacePath : TreeViewWithType<NamespacePath>
{
    public TreeViewNamespacePath(
        NamespacePath namespacePath,
        ICommonComponentRenderers commonComponentRenderers,
        IState<SolutionExplorerState> solutionExplorerStateWrap,
        IFileSystemProvider fileSystemProvider,
        bool isExpandable,
        bool isExpanded)
            : base(
                namespacePath,
                isExpandable,
                isExpanded)
    {
        CommonComponentRenderers = commonComponentRenderers;
        SolutionExplorerStateWrap = solutionExplorerStateWrap;
        FileSystemProvider = fileSystemProvider;
    }
 
    public ICommonComponentRenderers CommonComponentRenderers { get; }
    public IState<SolutionExplorerState> SolutionExplorerStateWrap { get; }
    public IFileSystemProvider FileSystemProvider { get; }

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
            CommonComponentRenderers.TreeViewNamespacePathRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(ITreeViewNamespacePathRendererType.TreeViewNamespacePath),
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
            var newChildren = new List<TreeViewNoType>();
            
            if (Item.AbsoluteFilePath.IsDirectory)
            {
                newChildren = await TreeViewHelper
                    .LoadChildrenForDirectoryAsync(this);
            }
            else
            {
                switch (Item.AbsoluteFilePath.ExtensionNoPeriod)
                {
                    case ExtensionNoPeriodFacts.DOT_NET_SOLUTION:
                        newChildren = await TreeViewHelper
                            .LoadChildrenForDotNetSolutionAsync(this);
                        break;
                    case ExtensionNoPeriodFacts.C_SHARP_PROJECT:
                        newChildren = await TreeViewHelper
                            .LoadChildrenForCSharpProjectAsync(this);
                        break;
                    case ExtensionNoPeriodFacts.RAZOR_MARKUP:
                        newChildren = await TreeViewHelper
                            .LoadChildrenForRazorMarkupAsync(this);
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
                    newChild.TreeViewNodeKey = oldChild.TreeViewNodeKey;
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
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> treeViews)
    {
        if (Item is null)
        {
            return;
        }
        
        if (Item.AbsoluteFilePath.ExtensionNoPeriod
            .EndsWith(ExtensionNoPeriodFacts.RAZOR_MARKUP))
        {
            TreeViewHelper.RazorMarkupFindRelatedFiles(
                this, 
                treeViews);
        }
    }
}