namespace BlazorStudio.ClassLib.UserInterface;

public class ArbitraryDimensionUnitList
{
    /// <summary>
    ///     'margin-left', 'margin-right', 'padding-top' etc...
    /// </summary>
    public string StyleAttributeName { get; set; }
    public List<DimensionUnit> DimensionUnits { get; set; } = new();
}