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
    
    public double ScrollWidth { get; set; }
    public double ScrollHeight { get; set; }
    
    public double VirtualizeRenderBlockWidth { get; set; }
    public double VirtualizeRenderBlockHeight { get; set; }

    /// <param name="items">The items to provide.</param>
    /// <param name="totalItemCount">The total item count in the source generating the items provided.</param>
    public VirtualizeCoordinateSystemResult(IEnumerable<T> items, 
        double scrollWidth,
        double scrollHeight, 
        double virtualizeRenderBlockWidth, 
        double virtualizeRenderBlockHeight)
    {
        Items = items;
        ScrollWidth = scrollWidth;
        ScrollHeight = scrollHeight;
        VirtualizeRenderBlockWidth = virtualizeRenderBlockWidth;
        VirtualizeRenderBlockHeight = virtualizeRenderBlockHeight;
    }
}