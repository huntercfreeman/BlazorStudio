using BlazorStudio.ClassLib.UserInterface;

namespace BlazorStudio.RazorLib.VirtualizeComponentExperiments;

public class VirtualizeBoundary
{
    public VirtualizeBoundary(VirtualizeBoundaryKind virtualizeBoundaryKind)
    {
        VirtualizeBoundaryKind = virtualizeBoundaryKind;
    }

    public VirtualizeBoundaryKind VirtualizeBoundaryKind { get; }
    public double WidthInPercentage { get; } = 100;
    public double OffsetFromTopInPixels { get; set; }
    public double HeightInPixels { get; set; }
    public DimensionsPositionKind DimensionsPositionKind { get; } = DimensionsPositionKind.Absolute;
}