using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.WorkspaceCase;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.SolutionExplorerCase;

[FeatureState]
public record SolutionExplorerState(IAbsoluteFilePath? SolutionAbsoluteFilePath)
{
    public SolutionExplorerState() : this(default(IAbsoluteFilePath?))
    {

    }
}