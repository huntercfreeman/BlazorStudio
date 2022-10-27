using BlazorStudio.ClassLib.Menu;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Menu;

public partial class MenuDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public MenuRecord MenuRecord { get; set; } = null!;

    private ElementReference? _menuDisplayElementReference;
    private MenuOptionRecord? _activeMenuOptionRecord;
    
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_menuDisplayElementReference is not null)
            {
                _menuDisplayElementReference.Value.FocusAsync();
            }
        }
        
        return base.OnAfterRenderAsync(firstRender);
    }
}