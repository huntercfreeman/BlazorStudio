using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Icons.Codicon;

public class IconBase : ComponentBase
{
    [Parameter]
    public int WidthInPixels { get; set; } = 16;
    [Parameter]
    public int HeightInPixels { get; set; } = 16;
}