using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Namespaces;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using BlazorTreeView.RazorLib;
using Fluxor;

namespace BlazorStudio.ClassLib.TreeViewImplementations.Helper;

public interface ITreeViewHelper
{
    public const char NAMESPACE_DELIMITER = '.';
    
    public ICommonComponentRenderers CommonComponentRenderers { get; }
    public IState<SolutionExplorerState> SolutionExplorerStateWrap { get; }

    public Task<List<TreeView>> LoadChildrenForDotNetSolutionAsync(
        NamespacePath dotNetSolutionNamespacePath);
    
    public Task<List<TreeView>> LoadChildrenForCSharpProjectAsync(
        NamespacePath cSharpProjectNamespacePath);

    public Task<List<TreeView>> LoadChildrenForDirectoryAsync(
        NamespacePath directoryNamespacePath);
}