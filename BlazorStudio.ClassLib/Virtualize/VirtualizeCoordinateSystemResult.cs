using BlazorStudio.ClassLib.UserInterface;

namespace BlazorStudio.ClassLib.Virtualize;

/// <typeparam name="T">The type of the context for each item in the list.</typeparam>
public class VirtualizeCoordinateSystemResult<T>
{
    public IEnumerable<T> Items { get; }
    
    public double ActualWidthOfResult { get; set; }
    public double ActualHeightOfResult { get; set; }

    public VirtualizeCoordinateSystemResult(IEnumerable<T> items,
        double actualWidthOfResult, 
        double actualHeightOfResult)
    {
        Items = items;
        ActualWidthOfResult = actualWidthOfResult;
        ActualHeightOfResult = actualHeightOfResult;
    }
}