namespace BlazorStudio.RazorLib.VirtualizeComponentExperiments;

public record VirtualizeRenderData<TItem>(VirtualizeItemWrapper<TItem>[] VirtualizeItemWrappers,
    VirtualizeBoundary TopVirtualizeBoundary,
    VirtualizeBoundary BottomVirtualizeBoundary)
{
    private static VirtualizeBoundary GetTopVirtualizeBoundaryInitialState() =>
        new(VirtualizeBoundaryKind.Top, 100, 0, 0);

    private static VirtualizeBoundary GetBottomVirtualizeBoundaryInitialState() =>
        new(VirtualizeBoundaryKind.Bottom, 100, 0, 0);

    public VirtualizeRenderData()
        : this(Array.Empty<VirtualizeItemWrapper<TItem>>(),
            GetTopVirtualizeBoundaryInitialState(),
            GetBottomVirtualizeBoundaryInitialState())
    {
    }
}