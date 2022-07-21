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
    /// The total item count in the source generating the items provided.
    /// </summary>
    public int TotalItemCount { get; }

    /// <summary>
    /// Used to render the Generic Item
    /// </summary>
    public Dimensions CoordinateSystemDimensions { get; set; } = null!;

    /// <param name="items">The items to provide.</param>
    /// <param name="totalItemCount">The total item count in the source generating the items provided.</param>
    public VirtualizeCoordinateSystemResult(IEnumerable<T> items, int totalItemCount, Dimensions coordinateSystemDimensions)
    {
        Items = items;
        TotalItemCount = totalItemCount;
        CoordinateSystemDimensions = coordinateSystemDimensions;
    }
}