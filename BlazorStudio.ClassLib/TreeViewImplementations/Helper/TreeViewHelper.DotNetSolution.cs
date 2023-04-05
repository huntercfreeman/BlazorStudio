using BlazorCommon.RazorLib.TreeView.TreeViewClasses;
using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static Task<List<TreeViewNoType>> LoadChildrenForDotNetSolutionAsync(
        TreeViewSolution treeViewSolution)
    {
        if (treeViewSolution.Item is null)
            return Task.FromResult<List<TreeViewNoType>>(new());

        var childProjects = treeViewSolution.Item.DotNetProjects
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

        return Task.FromResult(childProjects);
    }
}