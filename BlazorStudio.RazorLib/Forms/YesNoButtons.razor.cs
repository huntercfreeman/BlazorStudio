using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Forms;

public partial class YesNoButtons : ComponentBase
{
    [Parameter]
    public Action OnYesAction { get; set; } = null!;
    [Parameter]
    public Action OnNoAction { get; set; } = null!;
    [Parameter]
    public bool AutoFocusNoButton { get; set; }

    private ElementReference _noButtonElementReference;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            if (AutoFocusNoButton)
                await _noButtonElementReference.FocusAsync();

        await base.OnAfterRenderAsync(firstRender);
    }
}