using System.Collections.Immutable;
using BlazorStudio.ClassLib.Context;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.ContextCase;

[FeatureState]
public record ContextStates(ImmutableArray<ContextRecord> ActiveContextRecords)
{
    public ContextStates() : this(ImmutableArray<ContextRecord>.Empty)
    {
        ActiveContextRecords = new []
        {
            ContextFacts.GlobalContext
        }.ToImmutableArray();
    }
}