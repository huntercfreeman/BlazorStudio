namespace BlazorStudio.ClassLib.UserInterface;

public static class DimensionUnitOperationKindExtensionMethods
{
    public static string ToCssString(this DimensionUnitOperationKind dimensionUnitOperationKind)
    {
        return dimensionUnitOperationKind switch
        {
            DimensionUnitOperationKind.Add => "+",
            DimensionUnitOperationKind.Subtract => "-",
            DimensionUnitOperationKind.Multiply => "*",
            DimensionUnitOperationKind.Divide => "/",
            _ => throw new ApplicationException($"The {nameof(DimensionUnitOperationKind)}: '{dimensionUnitOperationKind}' was not recognized.")
        };
    }
}