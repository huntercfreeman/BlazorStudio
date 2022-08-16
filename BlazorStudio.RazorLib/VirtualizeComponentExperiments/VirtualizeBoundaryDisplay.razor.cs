using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.VirtualizeComponentExperiments;

public partial class VirtualizeBoundaryDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public VirtualizeBoundary VirtualizeBoundary { get; set; } = null!;
    [Parameter, EditorRequired]
    public Guid VirtualizeCoordinateSystemIdentifier { get; set; }
    
    public ElementReference? BoundaryElementReference;

    public string Id => $"vcs-{VirtualizeBoundary.DimensionsPositionKind}_{VirtualizeCoordinateSystemIdentifier}";
}