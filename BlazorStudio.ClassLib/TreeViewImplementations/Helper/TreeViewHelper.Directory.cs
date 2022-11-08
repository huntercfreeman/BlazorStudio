using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Namespaces;
using BlazorTreeView.RazorLib;

namespace BlazorStudio.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public async Task<List<TreeView>> LoadChildrenForDirectoryAsync(
        NamespacePath directoryNamespacePath)
    {
        var directoryAbsoluteFilePathString = directoryNamespacePath.AbsoluteFilePath
            .GetAbsoluteFilePathString();
        
        var childDirectoryTreeViewModels = Directory
            .GetDirectories(directoryAbsoluteFilePathString)
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(x, true);

                var namespaceString = directoryNamespacePath.Namespace +
                                      ITreeViewHelper.NAMESPACE_DELIMITER +
                                      absoluteFilePath.FileNameNoExtension;

                var namespacePath = new NamespacePath(
                    namespaceString,
                    absoluteFilePath);

                return (TreeView)new TreeViewNamespacePath(
                    namespacePath,
                    this)
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

                var namespaceString = directoryNamespacePath.Namespace;
                
                var namespacePath = new NamespacePath(
                    namespaceString,
                    absoluteFilePath);

                return (TreeView)new TreeViewNamespacePath(
                    namespacePath,
                    this)
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