using System.Collections.Immutable;

namespace BlazorStudio.RazorLib.VirtualizeComponents;

public class VirtualizeCoordinateSystemResult<T>
{
    public ImmutableArray<VirtualizeCoordinateSystemEntry<T>> ItemsToRender { get; set; }
    public VirtualizeCoordinateSystemBoundaryDimensions LeftBoundaryDimensions { get; set; }
    public VirtualizeCoordinateSystemBoundaryDimensions BottomBoundaryDimensions { get; set; }
    public VirtualizeCoordinateSystemBoundaryDimensions TopBoundaryDimensions { get; set; }
    public VirtualizeCoordinateSystemBoundaryDimensions RightBoundaryDimensions { get; set; }
}