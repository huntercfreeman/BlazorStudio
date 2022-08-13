using BlazorStudio.ClassLib.FileSystem.Interfaces;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.FolderExplorerCase;

[FeatureState]
public record FolderExplorerState(IAbsoluteFilePath? FolderAbsoluteFilePath)
{
    public FolderExplorerState() : this(default(IAbsoluteFilePath?))
    {

    }
}