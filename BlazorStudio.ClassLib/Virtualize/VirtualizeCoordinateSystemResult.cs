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
    /// Used to render CoordinateSystem viewport
    /// </summary>
    public Dimensions CoordinateSystemViewportDimensions { get; set; } = null!;
    
    public double ScrollWidth { get; set; }
    public double ScrollHeight { get; set; }
    
    public double ResultWidth { get; set; }
    public double ResultHeight { get; set; }

    /// <param name="items">The items to provide.</param>
    /// <param name="totalItemCount">The total item count in the source generating the items provided.</param>
    public VirtualizeCoordinateSystemResult(IEnumerable<T> items, 
        Dimensions coordinateSystemViewportDimensions,
        double scrollWidth,
        double scrollHeight)
    {
        Items = items;
        CoordinateSystemViewportDimensions = coordinateSystemViewportDimensions;
        ScrollWidth = scrollWidth;
        ScrollHeight = scrollHeight;
    }
}