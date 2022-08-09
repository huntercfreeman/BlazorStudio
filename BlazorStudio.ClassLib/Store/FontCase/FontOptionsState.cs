using BlazorStudio.ClassLib.UserInterface;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.FontCase;

[FeatureState]
public record FontOptionsState(DimensionUnit FontSize)
{
    public FontOptionsState() : this(new DimensionUnit()
    {
        DimensionUnitKind = DimensionUnitKind.Pixels,
        Value = 17
    })
    {
        
    }
}