using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Sequence;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.SolutionExplorerCase;

[FeatureState]
public record SolutionExplorerState(IAbsoluteFilePath? SolutionAbsoluteFilePath, SequenceKey SequenceKey)
{
    public SolutionExplorerState() : this(default(IAbsoluteFilePath?), SequenceKey.NewSequenceKey())
    {

    }
}