using BlazorCommon.RazorLib.TreeView.TreeViewClasses;
using BlazorStudio.ClassLib.DotNet;
using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static Task<List<TreeViewNoType>> LoadChildrenForDotNetSolutionAsync(
        TreeViewSolution treeViewSolution)
    {
        if (treeViewSolution.Item is null)
            return Task.FromResult<List<TreeViewNoType>>(new());

        var childSolutionFolders = treeViewSolution.Item.SolutionFolders
            .Select(x => (TreeViewNoType)new TreeViewSolutionFolder(
                x,
                treeViewSolution.BlazorStudioComponentRenderers,
                treeViewSolution.FileSystemProvider,
                treeViewSolution.EnvironmentProvider,
                true,
                false)
            {
                TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
            })
            .OrderBy(x => ((TreeViewSolutionFolder)x).Item.AbsoluteFilePath.FileNameNoExtension)
            .ToList();

        var childProjects = treeViewSolution.Item.DotNetProjects
            .Where(x => x.ProjectTypeGuid != DotNetSolutionFolder.SolutionFolderProjectTypeGuid)
            .Select(x =>
            {
                var namespacePath = new NamespacePath(
                    x.AbsoluteFilePath.FileNameNoExtension,
                    x.AbsoluteFilePath);
                
                return (TreeViewNoType)new TreeViewNamespacePath(
                    namespacePath,
                    treeViewSolution.BlazorStudioComponentRenderers,
                    treeViewSolution.FileSystemProvider,
                    treeViewSolution.EnvironmentProvider,
                    true,
                    false)
                {
                    TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                };
            })
            .OrderBy(x => ((TreeViewNamespacePath)x).Item.AbsoluteFilePath.FileNameNoExtension)
            .ToList();

        return Task.FromResult(
            childSolutionFolders
                .Union(childProjects)
                .ToList());
    }
}