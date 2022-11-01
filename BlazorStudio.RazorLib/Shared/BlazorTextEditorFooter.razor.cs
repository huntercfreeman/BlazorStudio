using BlazorStudio.ClassLib.Dimensions;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class BlazorTextEditorFooter : ComponentBase
{
    [Parameter, EditorRequired]
    public ElementDimensions FooterElementDimensions { get; set; } = null!;
}