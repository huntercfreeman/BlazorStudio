using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.VirtualizeComponents;

public partial class VirtualizeCoordinateSystemBoundary : ComponentBase
{
    [Parameter, EditorRequired]
    public VirtualizeCoordinateSystemBoundaryKind VirtualizeCoordinateSystemBoundaryKind { get; set; }

    private ElementReference? _virtualizeCoordinateSystemBoundaryElementReference;

    public ElementReference? VirtualizeCoordinateSystemBoundaryElementReference =>
        _virtualizeCoordinateSystemBoundaryElementReference;
}