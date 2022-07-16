namespace BlazorStudio.ClassLib.UserInterface;

public static class DimensionUnitKindExtensionMethods
{
    public static string ToCssString(this DimensionUnitKind dimensionUnitKind)
    {
        return dimensionUnitKind switch
        {  
            DimensionUnitKind.Pixels => "px",
            DimensionUnitKind.ViewportWidth => "vw",
            DimensionUnitKind.ViewportHeight => "vh",
            DimensionUnitKind.Percentage => "%",
            DimensionUnitKind.RootCharacterWidth => "rch",
            DimensionUnitKind.RootCharacterHeight => "rem",
            DimensionUnitKind.CharacterWidth => "ch",
            DimensionUnitKind.CharacterHeight => "em",
            _ => throw new ApplicationException($"The {nameof(dimensionUnitKind)}: '{dimensionUnitKind}' was not recognized.")
        };
    }
}