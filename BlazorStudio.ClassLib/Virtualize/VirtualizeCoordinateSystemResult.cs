using BlazorStudio.ClassLib.UserInterface;

namespace BlazorStudio.ClassLib.Virtualize;

/// <summary>
/// Copy of Microsoft's Virtualize component ItemsProviderResult
/// </summary>
/// <typeparam name="T">The type of the context for each item in the list.</typeparam>
public class VirtualizeCoordinateSystemResult<T>
{
    /// <summary>
    /// The items to provide.
    /// </summary>
    public IEnumerable<T> Items { get; }

    /// <summary>
    /// Used to render CoordinateSystem in its entirety NOT just the viewport
    /// </summary>
    public Dimensions CoordinateSystemDimensions { get; set; } = null!;
    
    public Dimensions LeftBoundaryDimensions { get; set; } = null!;
    public Dimensions RightBoundaryDimensions { get; set; } = null!;
    public Dimensions TopBoundaryDimensions { get; set; } = null!;
    public Dimensions BottomBoundaryDimensions { get; set; } = null!;

    /// <param name="items">The items to provide.</param>
    /// <param name="totalItemCount">The total item count in the source generating the items provided.</param>
    public VirtualizeCoordinateSystemResult(IEnumerable<T> items, 
        Dimensions coordinateSystemDimensions,
        Dimensions leftBoundaryDimensions,
        Dimensions rightBoundaryDimensions,
        Dimensions topBoundaryDimensions,
        Dimensions bottomBoundaryDimensions)
    {
        Items = items;
        CoordinateSystemDimensions = coordinateSystemDimensions;

        LeftBoundaryDimensions = leftBoundaryDimensions;
        RightBoundaryDimensions = rightBoundaryDimensions;
        TopBoundaryDimensions = topBoundaryDimensions;
        BottomBoundaryDimensions = bottomBoundaryDimensions;
    }
}