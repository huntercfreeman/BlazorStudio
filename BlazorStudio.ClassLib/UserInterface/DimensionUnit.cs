namespace BlazorStudio.ClassLib.UserInterface;

public class DimensionUnit
{
    public DimensionUnitKind DimensionUnitKind { get; set; }
    public DimensionUnitOperationKind DimensionUnitOperationKind { get; set; } = DimensionUnitOperationKind.Add;
    public double Value { get; set; }
}