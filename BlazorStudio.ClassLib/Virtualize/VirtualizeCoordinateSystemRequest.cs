namespace BlazorStudio.ClassLib.Virtualize;

public record VirtualizeCoordinateSystemRequest(double ScrollLeft,
    double ScrollTop,
    double ScrollWidth,
    double ScrollHeight,
    double ViewportWidth,
    double ViewportHeight,
    CancellationToken CancellationToken);