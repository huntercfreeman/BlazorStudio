using System.Text;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Virtualization;

public partial class VirtualizationBoundaryDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public VirtualizationBoundary VirtualizationBoundary { get; set; } = null!;
    [Parameter, EditorRequired]
    public string VirtualizationBoundaryDisplayId { get; set; } = null!;

    private string GetStyleCssString()
    {
        var styleBuilder = new StringBuilder();
        
        // Width
        {
            if (VirtualizationBoundary.WidthInPixels is null)
                styleBuilder.Append(" width: 100%;");
            else
                styleBuilder.Append($" width: {VirtualizationBoundary.WidthInPixels}px;");
        }
        
        // Height
        {
            if (VirtualizationBoundary.HeightInPixels is null)
                styleBuilder.Append(" height: 100%;");
            else
                styleBuilder.Append($" height: {VirtualizationBoundary.HeightInPixels}px;");
        }
        
        // Left
        {
            if (VirtualizationBoundary.LeftInPixels is null)
                styleBuilder.Append(" left: 100%;");
            else
                styleBuilder.Append($" left: {VirtualizationBoundary.LeftInPixels}px;");
        }
        
        // Top
        {
            if (VirtualizationBoundary.TopInPixels is null)
                styleBuilder.Append(" top: 100%;");
            else
                styleBuilder.Append($" top: {VirtualizationBoundary.TopInPixels}px;");
        }

        return styleBuilder.ToString();
    }
}