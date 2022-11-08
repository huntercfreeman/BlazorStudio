using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorTreeView.RazorLib;
using Fluxor;

namespace BlazorStudio.ClassLib.TreeViewImplementations;

public class TreeViewSolutionExplorer : TreeViewBase<NamespacePath>
{
    private readonly TreeViewRenderer _treeViewSolutionExplorerRenderer;
    private readonly TreeViewRenderer _treeViewExceptionRenderer;
    private readonly IState<SolutionExplorerState> _solutionExplorerStateWrap;

    public TreeViewSolutionExplorer(
        NamespacePath namespacePath, 
        TreeViewRenderer treeViewSolutionExplorerRenderer,
        TreeViewRenderer treeViewExceptionRenderer,
        IState<SolutionExplorerState> solutionExplorerStateWrap)
            : base(namespacePath)
    {
        _treeViewSolutionExplorerRenderer = treeViewSolutionExplorerRenderer;
        _treeViewExceptionRenderer = treeViewExceptionRenderer;
        _solutionExplorerStateWrap = solutionExplorerStateWrap;
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
        return _treeViewSolutionExplorerRenderer with {
            DynamicComponentParameters = new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewSolutionExplorer),
                    this
                },
            } 
        };
    }
    
    public override async Task LoadChildrenAsync()
    {
        if (Item is null)
            return;

        try
        {
            if (Item.AbsoluteFilePath.ExtensionNoPeriod !=
                ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
            {
                return;
            }

            var newChildren = await LoadChildrenForSolutionAsync();
        
            // Keep after this as common code
            
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
            
            Children = newChildren;
        }
        catch (Exception exception)
        {
            Children = new List<TreeView>
            {
                new TreeViewException(
                    exception,
                    _treeViewExceptionRenderer)
                {
                    IsExpandable = false,
                    IsExpanded = false,
                    Parent = this,
                    IndexAmongSiblings = 0,
                }
            };
        }
    }
    
    private async Task<List<TreeView>> LoadChildrenForSolutionAsync()
    {
        var indexAmongSiblings = 0;
        
        var solutionExplorerState = _solutionExplorerStateWrap.Value;

        if (solutionExplorerState.Solution is null)
            return new();

        var childProjects = solutionExplorerState.Solution.Projects
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(
                    x.FilePath, 
                    false);

                var namespacePath = new NamespacePath(
                    absoluteFilePath.FileNameNoExtension,
                    absoluteFilePath);
                
                return (TreeView)new TreeViewSolutionExplorer(
                    namespacePath,
                    _treeViewSolutionExplorerRenderer,
                    _treeViewExceptionRenderer,
                        _solutionExplorerStateWrap)
                {
                    Parent = this,
                    IndexAmongSiblings = indexAmongSiblings++,
                    IsExpandable = true,
                    IsExpanded = false,
                    TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                };
            })
            .ToList();

        return childProjects;
    }
}