using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.WorkspaceCase;

[FeatureState]
public record WorkspaceState(IAbsoluteFilePath? WorkspaceAbsoluteFilePath)
{
    public WorkspaceState() : this(default(IAbsoluteFilePath?))
    {

    }
}