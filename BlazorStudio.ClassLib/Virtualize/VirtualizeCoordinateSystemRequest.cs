namespace BlazorStudio.ClassLib.Virtualize;

/// <summary>
/// Copy of Microsoft's Virtualize component ItemsProviderRequest
/// </summary>
public readonly struct VirtualizeCoordinateSystemRequest
{
    /// <summary>
    /// The start index of the data segment requested.
    /// </summary>
    public int StartIndex { get; }

    /// <summary>
    /// The requested number of items to be provided. The actual number of provided items does not need to match
    /// this value.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// The <see cref="System.Threading.CancellationToken"/> used to relay cancellation of the request.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <param name="startIndex">The start index of the data segment requested.</param>
    /// <param name="count">The requested number of items to be provided.</param>
    /// <param name="cancellationToken">
    /// The <see cref="System.Threading.CancellationToken"/> used to relay cancellation of the request.
    /// </param>
    public VirtualizeCoordinateSystemRequest(int startIndex, int count, CancellationToken cancellationToken)
    {
        StartIndex = startIndex;
        Count = count;
        CancellationToken = cancellationToken;
    }
}