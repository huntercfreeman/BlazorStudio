namespace BlazorStudio.ClassLib.Dimensions;

public static class DimensionOperatorKindExtensions
{
    public static string GetStyleString(this DimensionOperatorKind dimensionOperatorKind)
    {
        return dimensionOperatorKind switch
        {
            DimensionOperatorKind.Add => "+",
            DimensionOperatorKind.Subtract => "-",
            DimensionOperatorKind.Multiply => "*",
            DimensionOperatorKind.Divide => "/",
            _ => throw new ApplicationException($"The {nameof(DimensionOperatorKind)}: '{dimensionOperatorKind}' was not recognized.")
        };
    }
}