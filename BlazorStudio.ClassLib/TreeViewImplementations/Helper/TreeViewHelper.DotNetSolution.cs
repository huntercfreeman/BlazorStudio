using BlazorCommon.RazorLib.TreeView.TreeViewClasses;
using BlazorStudio.ClassLib.FileSystem.Classes.FilePath;
using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static Task<List<TreeViewNoType>> LoadChildrenForDotNetSolutionAsync(
        TreeViewNamespacePath dotNetSolutionTreeView)
    {
        var solutionExplorerState = dotNetSolutionTreeView.SolutionExplorerStateWrap.Value;

        if (solutionExplorerState.Solution is null)
            return Task.FromResult<List<TreeViewNoType>>(new());

        var childProjects = solutionExplorerState.Solution.Projects
            .Select(x =>
            {
                var absoluteFilePath = new AbsoluteFilePath(
                    x.FilePath, 
                    false,
                    dotNetSolutionTreeView.EnvironmentProvider);

                var namespacePath = new NamespacePath(
                    absoluteFilePath.FileNameNoExtension,
                    absoluteFilePath);
                
                return (TreeViewNoType)new TreeViewNamespacePath(
                    namespacePath,
                    dotNetSolutionTreeView.CommonComponentRenderers,
                    dotNetSolutionTreeView.SolutionExplorerStateWrap,
                    dotNetSolutionTreeView.FileSystemProvider,
                    dotNetSolutionTreeView.EnvironmentProvider,
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