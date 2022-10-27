using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Menu;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Menu;

public partial class MenuDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public MenuRecord MenuRecord { get; set; } = null!;

    private ElementReference? _menuDisplayElementReference;
    
    /// <summary>
    /// First time the MenuDisplay opens
    /// the _activeMenuOptionRecordIndex == -1
    /// </summary>
    private int _activeMenuOptionRecordIndex = -1;
    
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

    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (MenuRecord.MenuOptions.Length == 0)
        {
            _activeMenuOptionRecordIndex = -1;
            return;
        }
        
        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_UP:
                if (_activeMenuOptionRecordIndex <= 0)
                {
                    _activeMenuOptionRecordIndex = MenuRecord.MenuOptions.Length - 1;
                }
                else
                {
                    _activeMenuOptionRecordIndex--;
                }
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN:
                if (_activeMenuOptionRecordIndex >= MenuRecord.MenuOptions.Length - 1)
                {
                    _activeMenuOptionRecordIndex = 0;
                }
                else
                {
                    _activeMenuOptionRecordIndex++;
                }
                break;
            case KeyboardKeyFacts.MovementKeys.HOME:
                _activeMenuOptionRecordIndex = 0;
                break;
            case KeyboardKeyFacts.MovementKeys.END:
                _activeMenuOptionRecordIndex = MenuRecord.MenuOptions.Length - 1;
                break;
        }
    }
}