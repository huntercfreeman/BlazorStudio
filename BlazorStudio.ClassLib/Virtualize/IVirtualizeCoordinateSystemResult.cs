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
    public double ActualWidthOfResult { get; }

    /// <summary>
    /// The height of the rendered result does not have to equal the requested height.
    ///
    /// This could mean it is greater than or less than the request.
    ///
    /// This is a requirement for variable height elements where the result might
    /// not always be a consistent ratio of height / element
    /// </summary>
    public double ActualHeightOfResult { get; }
}