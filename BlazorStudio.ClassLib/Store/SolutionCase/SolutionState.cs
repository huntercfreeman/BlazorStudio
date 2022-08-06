using Fluxor;
using Microsoft.CodeAnalysis.MSBuild;

namespace BlazorStudio.ClassLib.Store.SolutionCase;

[FeatureState]
public record SolutionState(MSBuildWorkspace? SolutionWorkspace)
{
    private SolutionState() : this(default(MSBuildWorkspace))
    {

    }
}