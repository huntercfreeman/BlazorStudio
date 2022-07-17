using Fluxor;

namespace BlazorStudio.ClassLib.Store.DropdownCase;

[FeatureState]
public record DropdownState(DropdownKey? ActiveDropdownKey)
{
    public DropdownState() : this(default(DropdownKey?))
    {

    }
}