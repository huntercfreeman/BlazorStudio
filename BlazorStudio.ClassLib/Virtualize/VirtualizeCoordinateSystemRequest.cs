namespace BlazorStudio.ClassLib.Virtualize;

public record VirtualizeCoordinateSystemRequest(
    double ScrollLeftInPixels,
    double ScrollTopInPixels,
    double ScrollWidthInPixels,
    double ScrollHeightInPixels,
    double ViewportWidthInPixels,
    double ViewportHeightInPixels,
    CancellationToken CancellationToken);