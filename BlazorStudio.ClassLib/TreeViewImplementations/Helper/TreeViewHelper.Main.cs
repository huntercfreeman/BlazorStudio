using BlazorStudio.ClassLib.CommonComponents;
using BlazorStudio.ClassLib.Store.SolutionExplorer;
using Fluxor;

namespace BlazorStudio.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public TreeViewHelper(
        ICommonComponentRenderers commonComponentRenderers,
        IState<SolutionExplorerState> solutionExplorerStateWrap)
    {
        CommonComponentRenderers = commonComponentRenderers;
        SolutionExplorerStateWrap = solutionExplorerStateWrap;
    }
    
    public ICommonComponentRenderers CommonComponentRenderers { get; }
    public IState<SolutionExplorerState> SolutionExplorerStateWrap { get; }
}