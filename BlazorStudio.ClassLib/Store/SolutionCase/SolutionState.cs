using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Fluxor;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace BlazorStudio.ClassLib.Store.SolutionCase;

[FeatureState]
public record SolutionState(MSBuildWorkspace? SolutionWorkspace, VisualStudioInstance? VisualStudioInstance,
    IAbsoluteFilePath? MsBuildAbsoluteFilePath)
{
    private SolutionState() : this(default(MSBuildWorkspace), default(VisualStudioInstance), default(IAbsoluteFilePath))
    {

    }
}