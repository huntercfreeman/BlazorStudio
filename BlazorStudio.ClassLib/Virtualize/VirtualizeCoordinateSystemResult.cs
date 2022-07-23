namespace BlazorStudio.ClassLib.Virtualize;

public record VirtualizeCoordinateSystemResult<T>(IEnumerable<T> ItemsWithType,
        IEnumerable<object?> ItemsUntyped,
        double ActualWidthOfResult,
        double ActualHeightOfResult)
    : IVirtualizeCoordinateSystemResult;