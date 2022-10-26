using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Forms;

public partial class YesNoButtons : ComponentBase
{
    private ElementReference _noButtonElementReference;
    [Parameter]
    public Action OnYesAction { get; set; } = null!;
    [Parameter]
    public Action OnNoAction { get; set; } = null!;
    [Parameter]
    public bool AutoFocusNoButton { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (AutoFocusNoButton)
                await _noButtonElementReference.FocusAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}