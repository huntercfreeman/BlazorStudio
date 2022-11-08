using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorTreeView.RazorLib;
using Fluxor;

namespace BlazorStudio.ClassLib.TreeViewImplementations;

public class TreeViewSolutionExplorer : TreeViewBase<NamespacePath>
{
    private const char NAMESPACE_DELIMITER = '.';
    
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
            var newChildren = new List<TreeView>();
            
            if (Item.AbsoluteFilePath.IsDirectory)
            {
                newChildren = await LoadChildrenForDirectoryAsync();
            }
            else
            {
                switch (Item.AbsoluteFilePath.ExtensionNoPeriod)
                {
                    case ExtensionNoPeriodFacts.DOT_NET_SOLUTION:
                        newChildren = await LoadChildrenForSolutionAsync();
                        break;
                    case ExtensionNoPeriodFacts.C_SHARP_PROJECT:
                        newChildren = await LoadChildrenForCSharpProjectAsync();
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
                    IsExpandable = true,
                    IsExpanded = false,
                    TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                };
            })
            .ToList();

        return childProjects;
    }
    
    private async Task<List<TreeView>> LoadChildrenForCSharpProjectAsync()
    {
        if (Item is null)
            return new();
        
        var parentDirectoryOfCSharpProject = (IAbsoluteFilePath)Item.AbsoluteFilePath.Directories
            .Last();

        var parentAbsoluteFilePathString = parentDirectoryOfCSharpProject
            .GetAbsoluteFilePathString();
        
        var hiddenFiles = HiddenFileFacts
            .GetHiddenFilesByContainerFileExtension(ExtensionNoPeriodFacts.C_SHARP_PROJECT);
        
        var childDirectoryTreeViewModels = Directory
            .GetDirectories(parentAbsoluteFilePathString)
            .Where(x => hiddenFiles.All(hidden => !x.EndsWith(hidden)))
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, true);

                var namespaceString = Item.Namespace +
                                      NAMESPACE_DELIMITER +
                                      absoluteFilePath.FileNameNoExtension;
                
                var namespacePath = new NamespacePath(
                    namespaceString,
                    absoluteFilePath);
                
                return new TreeViewSolutionExplorer(
                    namespacePath,
                    _treeViewSolutionExplorerRenderer,
                    _treeViewExceptionRenderer,
                    _solutionExplorerStateWrap)
                {
                    Parent = this,
                    IsExpandable = true,
                    IsExpanded = false,
                    TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                };
            });
        
        var uniqueDirectories = UniqueFileFacts
            .GetUniqueFilesByContainerFileExtension(
                ExtensionNoPeriodFacts.C_SHARP_PROJECT);
        
        var foundUniqueDirectories = new List<TreeViewSolutionExplorer>();
        var foundDefaultDirectories = new List<TreeViewSolutionExplorer>();

        foreach (var directoryTreeViewModel in childDirectoryTreeViewModels)
        {
            if (directoryTreeViewModel.Item is null)
                continue;

            if (uniqueDirectories.Any(unique => directoryTreeViewModel
                    .Item.AbsoluteFilePath.FileNameNoExtension == unique))
            {
                foundUniqueDirectories.Add(directoryTreeViewModel);
            }
            else
            {
                foundDefaultDirectories.Add(directoryTreeViewModel);
            }
        }
        
        foundUniqueDirectories = foundUniqueDirectories
            .OrderBy(x => x.Item?.AbsoluteFilePath.FileNameNoExtension ?? string.Empty)
            .ToList();

        foundDefaultDirectories = foundDefaultDirectories
            .OrderBy(x => x.Item?.AbsoluteFilePath.FileNameNoExtension ?? string.Empty)
            .ToList();
        
        var childFileTreeViewModels = Directory
            .GetFiles(parentAbsoluteFilePathString)
            .Where(x => !x.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, false);

                var namespaceString = Item.Namespace;
                
                var namespacePath = new NamespacePath(
                    namespaceString,
                    absoluteFilePath);
                
                return (TreeView)new TreeViewSolutionExplorer(
                    namespacePath,
                    _treeViewSolutionExplorerRenderer,
                    _treeViewExceptionRenderer,
                    _solutionExplorerStateWrap)
                {
                    Parent = this,
                    IsExpandable = true,
                    IsExpanded = false,
                    TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                };
            });
        
        return 
            foundUniqueDirectories
            .Union(foundDefaultDirectories)
            .Union(childFileTreeViewModels)
            .ToList();
    }
    
    private async Task<List<TreeView>> LoadChildrenForDirectoryAsync()
    {
        if (Item is null)
            return new();
        
        var directoryAbsoluteFilePathString = Item.AbsoluteFilePath
            .GetAbsoluteFilePathString();
        
        var childDirectoryTreeViewModels = Directory
            .GetDirectories(directoryAbsoluteFilePathString)
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, true);

                var namespaceString = Item.Namespace +
                                      NAMESPACE_DELIMITER +
                                      absoluteFilePath.FileNameNoExtension;

                var namespacePath = new NamespacePath(
                    namespaceString,
                    absoluteFilePath);

                return (TreeView)new TreeViewSolutionExplorer(
                    namespacePath,
                    _treeViewSolutionExplorerRenderer,
                    _treeViewExceptionRenderer,
                    _solutionExplorerStateWrap)
                {
                    Parent = this,
                    IsExpandable = true,
                    IsExpanded = false,
                    TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                };
            });
        
        var childFileTreeViewModels = Directory
            .GetFiles(directoryAbsoluteFilePathString)
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, false);

                var namespaceString = Item.Namespace;
                
                var namespacePath = new NamespacePath(
                    namespaceString,
                    absoluteFilePath);

                return (TreeView)new TreeViewSolutionExplorer(
                    namespacePath,
                    _treeViewSolutionExplorerRenderer,
                    _treeViewExceptionRenderer,
                    _solutionExplorerStateWrap)
                {
                    Parent = this,
                    IsExpandable = false,
                    IsExpanded = false,
                    TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                };
            });

        return childDirectoryTreeViewModels
            .Union(childFileTreeViewModels)
            .ToList();
    }
}