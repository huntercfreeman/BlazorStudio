namespace BlazorStudio.ClassLib.Dimensions;

public class DimensionUnit
{
    public double Value { get; set; }
    public DimensionUnitKind DimensionUnitKind { get; set; }
    public DimensionOperatorKind DimensionOperatorKind { get; set; } = DimensionOperatorKind.Add;
}