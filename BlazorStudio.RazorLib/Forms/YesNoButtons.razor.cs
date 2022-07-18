using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Forms;

public partial class YesNoButtons : ComponentBase
{
    [Parameter]
    public Action OnYesAction { get; set; } = null!;
    [Parameter]
    public Action OnNoAction { get; set; } = null!;
}