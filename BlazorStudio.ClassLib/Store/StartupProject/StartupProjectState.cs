using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.StartupProject;

[FeatureState]
public record StartupProjectState(IAbsoluteFilePath? ProjectAbsoluteFilePath)
{
    public StartupProjectState() : this(default(IAbsoluteFilePath))
    {
    }
}