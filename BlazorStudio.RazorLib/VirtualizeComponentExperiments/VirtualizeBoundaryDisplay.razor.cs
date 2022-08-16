using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.VirtualizeComponentExperiments;

public partial class VirtualizeBoundaryDisplay : ComponentBase
{
    [Parameter]
    public VirtualizeBoundary VirtualizeBoundary { get; set; } = null!;
    
    private ElementReference? _boundaryElementReference;
}