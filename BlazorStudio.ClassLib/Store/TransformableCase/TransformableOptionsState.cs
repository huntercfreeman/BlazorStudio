using BlazorStudio.ClassLib.UserInterface;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TransformableCase;

[FeatureState]
public record TransformableOptionsState(DimensionUnit ResizeHandleDimensionUnit)
{
    public TransformableOptionsState() 
        : this(new DimensionUnit()
                {
                    DimensionUnitKind = DimensionUnitKind.Pixels,
                    Value = 7
                })
    {
        
    }
}