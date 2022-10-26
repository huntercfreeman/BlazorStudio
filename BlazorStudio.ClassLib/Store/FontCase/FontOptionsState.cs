using BlazorStudio.ClassLib.UserInterface;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.FontCase;

[FeatureState]
public record FontOptionsState(DimensionUnit FontSize)
{
    public FontOptionsState() : this(new DimensionUnit
    {
        DimensionUnitKind = DimensionUnitKind.Pixels,
        Value = 17,
    })
    {
        if (OperatingSystem.IsWindows())
        {
            // On Windows I find that everything looks about 30% larger than when I use Linux so I reduce the size
            FontSize = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = 15,
            };
        }
    }
}