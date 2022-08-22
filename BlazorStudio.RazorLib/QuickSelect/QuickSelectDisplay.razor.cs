using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.QuickSelect;

public partial class QuickSelectDisplay : ComponentBase
{
    private ElementReference? _quickSelectDisplayElementReference;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_quickSelectDisplayElementReference is not null)
            {
                await _quickSelectDisplayElementReference.Value.FocusAsync();
            }
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }
}