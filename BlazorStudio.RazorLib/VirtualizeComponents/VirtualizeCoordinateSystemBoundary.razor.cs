using System.Text;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.VirtualizeComponents;

public partial class VirtualizeCoordinateSystemBoundary : ComponentBase
{
    [Parameter, EditorRequired]
    public VirtualizeCoordinateSystemBoundaryKind VirtualizeCoordinateSystemBoundaryKind { get; set; }
    [Parameter, EditorRequired]
    public VirtualizeCoordinateSystemBoundaryDimensions? VirtualizeCoordinateSystemBoundaryDimensions { get; set; }

    private ElementReference? _virtualizeCoordinateSystemBoundaryElementReference;

    private string CssStyle => GetCssStyle();
    
    public ElementReference? VirtualizeCoordinateSystemBoundaryElementReference =>
        _virtualizeCoordinateSystemBoundaryElementReference;
    
    private string GetCssStyle()
    {
        var styleBuilder = new StringBuilder();

        styleBuilder.Append($" width: {VirtualizeCoordinateSystemBoundaryDimensions?.Width ?? 0};");
        styleBuilder.Append($" height: {VirtualizeCoordinateSystemBoundaryDimensions?.Height ?? 0};");
        styleBuilder.Append($" left: {VirtualizeCoordinateSystemBoundaryDimensions?.Left ?? 0};");
        styleBuilder.Append($" top: {VirtualizeCoordinateSystemBoundaryDimensions?.Top ?? 0};");

        return styleBuilder.ToString();
    }
}