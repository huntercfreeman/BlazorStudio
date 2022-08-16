using BlazorStudio.ClassLib.UserInterface;

namespace BlazorStudio.RazorLib.VirtualizeComponentExperiments;

public record VirtualizeBoundary(VirtualizeBoundaryKind VirtualizeBoundaryKind,
    double WidthInPercentage,
    double OffsetFromTopInPixels,
    double HeightInPixels,
    DimensionsPositionKind DimensionsPositionKind = DimensionsPositionKind.Absolute);