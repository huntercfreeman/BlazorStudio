using System.Collections.Immutable;

namespace BlazorStudio.RazorLib.Virtualization;

public record VirtualizationEntry<T>( // Wraps the item the consumer of the component wants to render
    int Index, // The index of the item to render
    T Item, // the item itself
    double? WidthInPixels,
    double? HeightInPixels, // 
    double? LeftInPixels,
    double? TopInPixels); // 

public record VirtualizationScrollPosition(
    double ScrollLeftInPixels,
    double ScrollTopInPixels);

public record VirtualizationRequest(
    VirtualizationScrollPosition ScrollPosition,
    CancellationToken CancellationToken);
    
public record VirtualizationResult<T>(
    ImmutableArray<VirtualizationEntry<T>> Entries,
    VirtualizationBoundary LeftVirtualizationBoundary,
    VirtualizationBoundary RightVirtualizationBoundary,
    VirtualizationBoundary TopVirtualizationBoundary,
    VirtualizationBoundary BottomVirtualizationBoundary);
    
public record VirtualizationBoundary(
    double? WidthInPixels,
    double? HeightInPixels,
    double? LeftInPixels,
    double? TopInPixels);