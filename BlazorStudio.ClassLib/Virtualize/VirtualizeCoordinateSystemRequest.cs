namespace BlazorStudio.ClassLib.Virtualize;

public class VirtualizeCoordinateSystemRequest<T>
{
    public double ScrollLeft { get; set; }
    public double ScrollTop { get; set; }
    public double ScrollWidth { get; set; }
    public double ScrollHeight { get; set; }
    public double ViewportWidth { get; set; }
    public double ViewportHeight { get; set; }

    public VirtualizeCoordinateSystemResult<T>? Result { get; set; }
    
    /// <summary>
    /// The <see cref="System.Threading.CancellationToken"/> used to relay cancellation of the request.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <param name="startIndex">The start index of the data segment requested.</param>
    /// <param name="count">The requested number of items to be provided.</param>
    /// <param name="cancellationToken">
    /// The <see cref="System.Threading.CancellationToken"/> used to relay cancellation of the request.
    /// </param>
    public VirtualizeCoordinateSystemRequest(
        double scrollLeft, 
        double scrollTop,
        double scrollWidth,
        double scrollHeight,
        double viewportWidth,
        double viewportHeight,
        CancellationToken cancellationToken)
    {
        ScrollLeft = scrollLeft;
        ScrollTop = scrollTop;
        ScrollWidth = scrollWidth;
        ScrollHeight = scrollHeight;
        ViewportWidth = viewportWidth;
        ViewportHeight = viewportHeight;
        CancellationToken = cancellationToken;
    }
}