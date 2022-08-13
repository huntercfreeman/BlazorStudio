using BlazorStudio.ClassLib.FileSystem.Classes;
using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Sequence;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.SolutionExplorerCase;

[FeatureState]
public record SolutionExplorerState(AbsoluteFilePathDotNet? SolutionAbsoluteFilePath, SequenceKey SequenceKey)
{
    public SolutionExplorerState() : this(default(AbsoluteFilePathDotNet?), SequenceKey.NewSequenceKey())
    {

    }
}