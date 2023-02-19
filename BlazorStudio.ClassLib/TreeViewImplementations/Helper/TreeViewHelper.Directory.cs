using BlazorALaCarte.TreeView;
using BlazorALaCarte.TreeView.BaseTypes;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static Task<List<TreeViewNoType>> LoadChildrenForDirectoryAsync(
        TreeViewNamespacePath directoryTreeView)
    {
        if (directoryTreeView.Item is null)
            return Task.FromResult<List<TreeViewNoType>>(new());
        
        var directoryAbsoluteFilePathString = directoryTreeView.Item.AbsoluteFilePath
            .GetAbsoluteFilePathString();
        
        var childDirectoryTreeViewModels = 
            directoryTreeView.FileSystemProvider
                .DirectoryGetDirectories(directoryAbsoluteFilePathString)
                .OrderBy(filePathString => filePathString)
                .Select(x =>
                {
                    var absoluteFilePath = new AbsoluteFilePath(x, true);

                    var namespaceString = directoryTreeView.Item.Namespace +
                                          NAMESPACE_DELIMITER +
                                          absoluteFilePath.FileNameNoExtension;

                    var namespacePath = new NamespacePath(
                        namespaceString,
                        absoluteFilePath);

                    return (TreeViewNoType)new TreeViewNamespacePath(
                        namespacePath,
                        directoryTreeView.CommonComponentRenderers,
                        directoryTreeView.SolutionExplorerStateWrap,
                        directoryTreeView.FileSystemProvider,
                        true,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                    };
                });
        
        var childFileTreeViewModels = 
            
            directoryTreeView.FileSystemProvider
                .DirectoryGetFiles(directoryAbsoluteFilePathString)
                .OrderBy(filePathString => filePathString)
                .Select(x =>
                {
                    var absoluteFilePath = new AbsoluteFilePath(x, false);

                    var namespaceString = directoryTreeView.Item.Namespace;
                    
                    var namespacePath = new NamespacePath(
                        namespaceString,
                        absoluteFilePath);

                    return (TreeViewNoType)new TreeViewNamespacePath(
                        namespacePath,
                        directoryTreeView.CommonComponentRenderers,
                        directoryTreeView.SolutionExplorerStateWrap,
                        directoryTreeView.FileSystemProvider,
                        false,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                    };
                }).ToList();

        var copyOfChildrenToFindRelatedFiles = new List<TreeViewNoType>(childFileTreeViewModels);
        
        foreach (var child in childFileTreeViewModels)
        {
            child.RemoveRelatedFilesFromParent(
                copyOfChildrenToFindRelatedFiles);
        }

        // The parent directory gets what is left over after the
        // children take their respective 'code behinds'
        childFileTreeViewModels = copyOfChildrenToFindRelatedFiles;

        return Task.FromResult(childDirectoryTreeViewModels
            .Union(childFileTreeViewModels)
            .ToList());
    }
    
    public static Task<List<TreeViewNoType>> LoadChildrenForDirectoryAsync(
        TreeViewAbsoluteFilePath directoryTreeView)
    {
        if (directoryTreeView.Item is null)
            return Task.FromResult<List<TreeViewNoType>>(new());
        
        var directoryAbsoluteFilePathString = directoryTreeView.Item
            .GetAbsoluteFilePathString();
        
        var childDirectoryTreeViewModels = 
            
            directoryTreeView.FileSystemProvider
                .DirectoryGetDirectories(directoryAbsoluteFilePathString)
                .OrderBy(filePathString => filePathString)
                .Select(x =>
                {
                    var absoluteFilePath = new AbsoluteFilePath(x, true);

                    return (TreeViewNoType)new TreeViewAbsoluteFilePath(
                        absoluteFilePath,
                        directoryTreeView.CommonComponentRenderers,
                        directoryTreeView.FileSystemProvider,
                        true,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                    };
                });
        
        var childFileTreeViewModels = 
            
            directoryTreeView.FileSystemProvider
                .DirectoryGetFiles(directoryAbsoluteFilePathString)
                .OrderBy(filePathString => filePathString)
                .Select(x =>
                {
                    var absoluteFilePath = new AbsoluteFilePath(x, false);

                    return (TreeViewNoType)new TreeViewAbsoluteFilePath(
                        absoluteFilePath,
                        directoryTreeView.CommonComponentRenderers,
                        directoryTreeView.FileSystemProvider,
                        false,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                    };
                });

        return Task.FromResult(childDirectoryTreeViewModels
            .Union(childFileTreeViewModels)
            .ToList());
    }
}