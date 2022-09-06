namespace BlazorStudio.RazorLib.VirtualizeComponents;

public record VirtualizeCoordinateSystemScrollPosition(
    double ScrollLeft, 
    double ScrollTop,
    CancellationToken CancellationToken);