using BlazorStudio.ClassLib.FileConstants;
using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Namespaces;
using BlazorTextEditor.RazorLib.TreeView;

namespace BlazorStudio.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static async Task<List<TreeView>> LoadChildrenForCSharpProjectAsync(
        TreeViewNamespacePath cSharpProjectTreeView)
    {
        if (cSharpProjectTreeView.Item is null)
            return new();
        
        var parentDirectoryOfCSharpProject = (IAbsoluteFilePath)
            cSharpProjectTreeView.Item.AbsoluteFilePath.Directories
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

                var namespaceString = cSharpProjectTreeView.Item.Namespace +
                                      NAMESPACE_DELIMITER +
                                      absoluteFilePath.FileNameNoExtension;
                
                return new TreeViewNamespacePath(
                    new NamespacePath(
                        namespaceString,
                        absoluteFilePath),
                    cSharpProjectTreeView.CommonComponentRenderers,
                    cSharpProjectTreeView.SolutionExplorerStateWrap)
                {
                    IsExpandable = true,
                    IsExpanded = false,
                    TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                };
            });
        
        var uniqueDirectories = UniqueFileFacts
            .GetUniqueFilesByContainerFileExtension(
                ExtensionNoPeriodFacts.C_SHARP_PROJECT);
        
        var foundUniqueDirectories = new List<TreeViewNamespacePath>();
        var foundDefaultDirectories = new List<TreeViewNamespacePath>();

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

                var namespaceString = cSharpProjectTreeView.Item.Namespace;
                
                return (TreeView)new TreeViewNamespacePath(
                    new NamespacePath(
                        namespaceString,
                        absoluteFilePath),
                    cSharpProjectTreeView.CommonComponentRenderers,
                    cSharpProjectTreeView.SolutionExplorerStateWrap)
                {
                    IsExpandable = false,
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
}