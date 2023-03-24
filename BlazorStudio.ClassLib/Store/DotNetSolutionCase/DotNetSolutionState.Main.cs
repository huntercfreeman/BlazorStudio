using BlazorStudio.ClassLib.DotNet;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.DotNetSolutionCase;

[FeatureState]
public partial record DotNetSolutionState(
    DotNetSolution? DotNetSolution)
{
    private DotNetSolutionState() : this(default(DotNetSolution?))
    {
    }
}