using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.Namespaces;
using BlazorTreeView.RazorLib;

namespace BlazorStudio.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static async Task<List<TreeView>> LoadChildrenForDotNetSolutionAsync(
        TreeViewNamespacePath dotNetSolutionTreeView)
    {
        var solutionExplorerState = dotNetSolutionTreeView.SolutionExplorerStateWrap.Value;

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
                
                return (TreeView)new TreeViewNamespacePath(
                    namespacePath,
                    dotNetSolutionTreeView.CommonComponentRenderers,
                    dotNetSolutionTreeView.SolutionExplorerStateWrap)
                {
                    IsExpandable = true,
                    IsExpanded = false,
                    TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                };
            })
            .OrderBy(x => ((TreeViewNamespacePath)x).Item.AbsoluteFilePath.FileNameNoExtension)
            .ToList();

        return childProjects;
    }
}