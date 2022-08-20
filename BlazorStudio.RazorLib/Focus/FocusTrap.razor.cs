using BlazorStudio.RazorLib.Dialog;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Focus;

/// <summary>
/// Used typically in situations where a "quick select" input Blazor component
/// is empty in order to ensure focus is not lost from the "quick select".
///
/// See <see cref="DialogQuickSelectOverlay"/>
/// </summary>
public partial class FocusTrap : ComponentBase
{
    private ElementReference _focusTrapElementReference;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await FocusAsync();
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task FocusAsync()
    {
        await _focusTrapElementReference.FocusAsync();
    }
}