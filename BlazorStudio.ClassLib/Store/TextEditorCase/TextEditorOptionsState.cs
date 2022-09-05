using BlazorStudio.ClassLib.UserInterface;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TextEditorCase;

[FeatureState]
public record TextEditorOptionsState(DimensionUnit FontSize)
{
    public TextEditorOptionsState() : this(new DimensionUnit()
    {
        DimensionUnitKind = DimensionUnitKind.Pixels,
        Value = 21
    })
    {
        if (OperatingSystem.IsWindows())
        {
            // On Windows I find that everything looks about 30% larger than when I use Linux so I reduce the size
            FontSize = new DimensionUnit()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = 18
            };
        }
    }
}