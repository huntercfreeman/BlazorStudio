using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Namespaces;
using BlazorTreeView.RazorLib;

namespace BlazorStudio.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static async Task<List<TreeView>> LoadChildrenForDirectoryAsync(
        TreeViewNamespacePath treeViewNamespacePath)
    {
        if (treeViewNamespacePath.Item is null)
            return new();
        
        var directoryAbsoluteFilePathString = treeViewNamespacePath.Item.AbsoluteFilePath
            .GetAbsoluteFilePathString();
        
        var childDirectoryTreeViewModels = Directory
            .GetDirectories(directoryAbsoluteFilePathString)
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, true);

                var namespaceString = treeViewNamespacePath.Item.Namespace +
                                      TreeViewHelper.NAMESPACE_DELIMITER +
                                      absoluteFilePath.FileNameNoExtension;

                var namespacePath = new NamespacePath(
                    namespaceString,
                    absoluteFilePath);

                return (TreeView)new TreeViewNamespacePath(
                    namespacePath,
                    treeViewNamespacePath.CommonComponentRenderers,
                    treeViewNamespacePath.SolutionExplorerStateWrap)
                {
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

                var namespaceString = treeViewNamespacePath.Item.Namespace;
                
                var namespacePath = new NamespacePath(
                    namespaceString,
                    absoluteFilePath);

                return (TreeView)new TreeViewNamespacePath(
                    namespacePath,
                    treeViewNamespacePath.CommonComponentRenderers,
                    treeViewNamespacePath.SolutionExplorerStateWrap)
                {
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