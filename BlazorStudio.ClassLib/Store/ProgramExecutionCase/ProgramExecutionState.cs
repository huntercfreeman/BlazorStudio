using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.ProgramExecutionCase;

[FeatureState]
public record ProgramExecutionState(IAbsoluteFilePath? StartupProjectAbsoluteFilePath)
{
    private ProgramExecutionState() : this(default(IAbsoluteFilePath))
    {
        
    }

    public record SetStartupProjectAbsoluteFilePathAction(IAbsoluteFilePath? StartupProjectAbsoluteFilePath);
}