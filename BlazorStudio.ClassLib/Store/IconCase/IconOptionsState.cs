using BlazorStudio.ClassLib.UserInterface;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.IconCase;

[FeatureState]
public record IconOptionsState(DimensionUnit IconSize)
{
    public IconOptionsState() : this(new DimensionUnit()
    {
        DimensionUnitKind = DimensionUnitKind.Pixels,
        Value = 24,
    })
    {
        if (OperatingSystem.IsWindows())
        {
            // On Windows I find that everything looks about 30% larger than when I use Linux so I reduce the size
            IconSize = new DimensionUnit()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = 18,
            };
        }
    }
}