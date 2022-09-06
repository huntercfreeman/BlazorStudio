using BlazorStudio.RazorLib.VirtualizeComponents;
using Microsoft.AspNetCore.Components;

namespace TestingThings.Server.Pages;

public partial class DebugVirtualizeCoordinateSystemBoundaryDimensions : ComponentBase
{
    [Parameter, EditorRequired]
    public VirtualizeCoordinateSystemBoundaryDimensions VirtualizeCoordinateSystemBoundaryDimensions { get; set; }
    [Parameter, EditorRequired]
    public string Kind { get; set; } = null!;
    [Parameter, EditorRequired]
    public string Color { get; set; } = null!;
}