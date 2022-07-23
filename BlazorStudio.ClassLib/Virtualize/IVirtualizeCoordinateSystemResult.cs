namespace BlazorStudio.ClassLib.Virtualize;

public interface IVirtualizeCoordinateSystemResult
{
    /// <summary>
    /// Untyped to allow for the Reducer to have a
    /// method parameter (method generics I don't believe work for Reducers).
    /// </summary>
    public IEnumerable<object?> ItemsUntyped { get; }

    /// <summary>
    /// The width of the rendered result does not have to equal the requested width.
    ///
    /// This could mean it is greater than or less than the request.
    ///
    /// This is a requirement for variable width elements where the result might
    /// not always be a consistent ratio of width / element
    /// </summary>
    public double WidthOfResultInPixels { get; }

    /// <summary>
    /// The height of the rendered result does not have to equal the requested height.
    ///
    /// This could mean it is greater than or less than the request.
    ///
    /// This is a requirement for variable height elements where the result might
    /// not always be a consistent ratio of height / element
    /// </summary>
    public double HeightOfResultInPixels { get; }

    /// <summary>
    /// Total width of all elements in pixels
    ///
    /// Used to render horizontal scrollbar
    /// </summary>
    public double TotalWidthInPixels { get; }

    /// <summary>
    /// Total height of all elements in pixels
    ///
    /// Used to render vertical scrollbar
    /// </summary>
    public double TotalHeightInPixels { get; }
}