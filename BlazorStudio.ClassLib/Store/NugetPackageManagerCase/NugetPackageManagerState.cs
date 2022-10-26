using BlazorStudio.ClassLib.Sequence;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.NugetPackageManagerCase;

[FeatureState]
public record NugetPackageManagerState(SequenceKey FocusRequestedSequenceKey)
{
    public NugetPackageManagerState() : this(SequenceKey.NewSequenceKey())
    {
    }
}