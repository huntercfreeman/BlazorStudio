namespace BlazorStudio.ClassLib.Virtualize;

public record VirtualizeCoordinateSystemResult<T>(IEnumerable<T> ItemsWithType,
        IEnumerable<object?> ItemsUntyped,
        double WidthOfResultInPixels,
        double HeightOfResultInPixels,
        double TotalWidthInPixels,
        double TotalHeightInPixels)
    : IVirtualizeCoordinateSystemResult;
