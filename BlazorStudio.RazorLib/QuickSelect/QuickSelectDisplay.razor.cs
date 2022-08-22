using BlazorStudio.ClassLib.Store.QuickSelectCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.QuickSelect;

public partial class QuickSelectDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public QuickSelectState QuickSelectState { get; set; } = null!;
    
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

    private void HandleOnKeyDown()
    {
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine("QuickSelectDisplay HandleOnKeyDown");
        }
    }
    
    private void HandleOnFocusOut()
    {
        Dispatcher.Dispatch(new SetQuickSelectStateAction(QuickSelectState with
        {
            IsDisplayed = false
        }));
    }
}