using BlazorStudio.ClassLib.UserInterface;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.IconCase;

[FeatureState]
public record IconOptionsState(DimensionUnit IconSize)
{
    public IconOptionsState() : this(new DimensionUnit()
    {
        DimensionUnitKind = DimensionUnitKind.Pixels,
        Value = 24
    })
    {
        
    }
}