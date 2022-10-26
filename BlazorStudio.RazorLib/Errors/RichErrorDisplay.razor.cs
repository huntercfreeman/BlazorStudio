using BlazorStudio.ClassLib.Errors;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Errors;

public partial class RichErrorDisplay : ComponentBase
{
    [Parameter]
    [EditorRequired]
    public RichErrorModel RichErrorModel { get; set; } = null!;
}