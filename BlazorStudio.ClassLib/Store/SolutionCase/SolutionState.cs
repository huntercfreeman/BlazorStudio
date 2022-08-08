using Fluxor;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace BlazorStudio.ClassLib.Store.SolutionCase;

[FeatureState]
public record SolutionState(MSBuildWorkspace? SolutionWorkspace, VisualStudioInstance? VisualStudioInstance)
{
    private SolutionState() : this(default(MSBuildWorkspace), default(VisualStudioInstance))
    {

    }
}