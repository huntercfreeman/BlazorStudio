using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.QuickSelectCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.QuickSelect;

public partial class QuickSelectDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public QuickSelectState QuickSelectState { get; set; } = null!;
    
    private ElementReference? _quickSelectDisplayElementReference;
    private int _activeEntryIndex;
    
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

    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        var localQuickSelectState = QuickSelectState;
        if (keyboardEventArgs.Key == "d")
        {
            if (_activeEntryIndex < localQuickSelectState.QuickSelectItems.Length - 1)
            {
                _activeEntryIndex++;
            }
            else
            {
                _activeEntryIndex = 0;
            }
        }
        else if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceKeys.ENTER_CODE)
        {
            if (_activeEntryIndex < localQuickSelectState.QuickSelectItems.Length)
            {
                HandleOnFocusOut();
                
                await localQuickSelectState.OnItemSelectedFunc.Invoke(localQuickSelectState.QuickSelectItems[_activeEntryIndex]);
            }
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