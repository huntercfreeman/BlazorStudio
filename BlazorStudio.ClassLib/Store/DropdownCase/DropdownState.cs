using System.Collections.Immutable;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.DropdownCase;

[FeatureState]
public record DropdownState(ImmutableList<DropdownKey> ActiveDropdownKeys)
{
    public DropdownState() : this(ImmutableList<DropdownKey>.Empty)
    {

    }
}