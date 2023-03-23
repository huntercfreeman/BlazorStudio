using BlazorCommon.RazorLib.TreeView.TreeViewClasses;
using BlazorStudio.ClassLib.FileSystem.Classes.FilePath;
using BlazorStudio.ClassLib.Namespaces;

namespace BlazorStudio.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static Task<List<TreeViewNoType>> LoadChildrenForDotNetSolutionAsync(
        TreeViewNamespacePath dotNetSolutionTreeView)
    {
        return Task.FromResult(new List<TreeViewNoType>());
    }
}