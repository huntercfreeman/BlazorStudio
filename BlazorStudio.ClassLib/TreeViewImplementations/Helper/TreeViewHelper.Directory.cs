using BlazorALaCarte.TreeView;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static async Task<List<TreeView>> LoadChildrenForDirectoryAsync(
        TreeViewNamespacePath directoryTreeView)
    {
        if (directoryTreeView.Item is null)
            return new();
        
        var directoryAbsoluteFilePathString = directoryTreeView.Item.AbsoluteFilePath
            .GetAbsoluteFilePathString();
        
        var childDirectoryTreeViewModels = Directory
            .GetDirectories(directoryAbsoluteFilePathString)
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, true);

                var namespaceString = directoryTreeView.Item.Namespace +
                                      TreeViewHelper.NAMESPACE_DELIMITER +
                                      absoluteFilePath.FileNameNoExtension;

                var namespacePath = new NamespacePath(
                    namespaceString,
                    absoluteFilePath);

                return (TreeView)new TreeViewNamespacePath(
                    namespacePath,
                    directoryTreeView.CommonComponentRenderers,
                    directoryTreeView.SolutionExplorerStateWrap,
                    true,
                    false)
                {
                    TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                };
            });
        
        var childFileTreeViewModels = Directory
            .GetFiles(directoryAbsoluteFilePathString)
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, false);

                var namespaceString = directoryTreeView.Item.Namespace;
                
                var namespacePath = new NamespacePath(
                    namespaceString,
                    absoluteFilePath);

                return (TreeView)new TreeViewNamespacePath(
                    namespacePath,
                    directoryTreeView.CommonComponentRenderers,
                    directoryTreeView.SolutionExplorerStateWrap,
                    false,
                    false)
                {
                    TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                };
            }).ToList();

        var copyOfChildrenToFindRelatedFiles = new List<TreeView>(childFileTreeViewModels);
        
        foreach (var child in childFileTreeViewModels)
        {
            child.RemoveRelatedFilesFromParent(
                copyOfChildrenToFindRelatedFiles);
        }

        // The parent directory gets what is left over after the
        // children take their respective 'code behinds'
        childFileTreeViewModels = copyOfChildrenToFindRelatedFiles;

        return childDirectoryTreeViewModels
            .Union(childFileTreeViewModels)
            .ToList();
    }
    
    public static async Task<List<TreeView>> LoadChildrenForDirectoryAsync(
        TreeViewAbsoluteFilePath directoryTreeView)
    {
        if (directoryTreeView.Item is null)
            return new();
        
        var directoryAbsoluteFilePathString = directoryTreeView.Item
            .GetAbsoluteFilePathString();
        
        var childDirectoryTreeViewModels = Directory
            .GetDirectories(directoryAbsoluteFilePathString)
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, true);

                return (TreeView)new TreeViewAbsoluteFilePath(
                    absoluteFilePath,
                    directoryTreeView.CommonComponentRenderers,
                    true,
                    false)
                {
                    TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                };
            });
        
        var childFileTreeViewModels = Directory
            .GetFiles(directoryAbsoluteFilePathString)
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, false);

                return (TreeView)new TreeViewAbsoluteFilePath(
                    absoluteFilePath,
                    directoryTreeView.CommonComponentRenderers,
                    false,
                    false)
                {
                    TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                };
            });

        return childDirectoryTreeViewModels
            .Union(childFileTreeViewModels)
            .ToList();
    }
}