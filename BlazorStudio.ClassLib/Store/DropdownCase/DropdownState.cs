using Fluxor;

namespace BlazorStudio.ClassLib.Store.DropdownCase;

[FeatureState]
public record DropdownState(DropdownKey? ActiveDropdownKey = null!);

public record DropdownKey(Guid Guid)
{
    public static DropdownKey NewDropdownKey()
    {
        return new DropdownKey(Guid.NewGuid());
    }
}