namespace BlazorStudio.ClassLib.Virtualize;

public struct VirtualizeCoordinateSystemRequest
{
    public double ScrollLeft { get; set; }
    public double ScrollTop { get; set; }
    public double ViewportWidthResult { get; set; }
    public double ViewportHeightResult { get; set; }
    public double ScrollWidth { get; set; }
    public double ScrollHeight { get; set; }

    /// <summary>
    /// The <see cref="System.Threading.CancellationToken"/> used to relay cancellation of the request.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <param name="startIndex">The start index of the data segment requested.</param>
    /// <param name="count">The requested number of items to be provided.</param>
    /// <param name="cancellationToken">
    /// The <see cref="System.Threading.CancellationToken"/> used to relay cancellation of the request.
    /// </param>
    public VirtualizeCoordinateSystemRequest(double scrollLeft, 
        double scrollTop,
        double scrollWidth,
        double scrollHeight,
        double viewportWidthResult,
        double viewportHeightResult,
        CancellationToken cancellationToken)
    {
        ScrollLeft = scrollLeft;
        ScrollTop = scrollTop;
        ScrollWidth = scrollWidth;
        ScrollHeight = scrollHeight;
        ViewportWidthResult = viewportWidthResult;
        ViewportHeightResult = viewportHeightResult;
        CancellationToken = cancellationToken;
    }
}