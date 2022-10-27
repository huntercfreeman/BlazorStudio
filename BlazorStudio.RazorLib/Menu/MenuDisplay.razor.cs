using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Store.DropdownCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Menu;

public partial class MenuDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [CascadingParameter(Name="ReturnFocusToParentAction")]
    public Action? ReturnFocusToParentAction { get; set; }
    [CascadingParameter]
    public DropdownKey? DropdownKey { get; set; }
    
    [Parameter, EditorRequired]
    public MenuRecord MenuRecord { get; set; } = null!;
    [Parameter, EditorRequired]
    public int InitialActiveMenuOptionRecordIndex { get; set; } = -1;

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
            _activeMenuOptionRecordIndex = InitialActiveMenuOptionRecordIndex;

            if (_activeMenuOptionRecordIndex == -1 &&
                _menuDisplayElementReference is not null)
            {
                _menuDisplayElementReference.Value.FocusAsync();
            }
            else
            {
                InvokeAsync(StateHasChanged);
            }
        }
        
        return base.OnAfterRenderAsync(firstRender);
    }

    private void RestoreFocusToThisMenu()
    {
        if (_activeMenuOptionRecordIndex == -1)
        {
            if (_menuDisplayElementReference is not null)
            {
                _menuDisplayElementReference.Value.FocusAsync();
            }
        }

        InvokeAsync(StateHasChanged);
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
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_LEFT:
                if (DropdownKey is not null &&
                    ReturnFocusToParentAction is not null)
                {
                    Dispatcher.Dispatch(new RemoveActiveDropdownKeyAction(DropdownKey));
                    ReturnFocusToParentAction.Invoke();
                }
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN:
                if (_activeMenuOptionRecordIndex >= MenuRecord.MenuOptions.Length - 1)
                    _activeMenuOptionRecordIndex = 0;
                else
                    _activeMenuOptionRecordIndex++;
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_UP:
                if (_activeMenuOptionRecordIndex <= 0)
                    _activeMenuOptionRecordIndex = MenuRecord.MenuOptions.Length - 1;
                else
                    _activeMenuOptionRecordIndex--;
                break;
            case KeyboardKeyFacts.MovementKeys.HOME:
                _activeMenuOptionRecordIndex = 0;
                break;
            case KeyboardKeyFacts.MovementKeys.END:
                _activeMenuOptionRecordIndex = MenuRecord.MenuOptions.Length - 1;
                break;
            case KeyboardKeyFacts.MetaKeys.ESCAPE:
                if (DropdownKey is not null)
                    Dispatcher.Dispatch(new RemoveActiveDropdownKeyAction(DropdownKey));

                if (ReturnFocusToParentAction is not null)
                    ReturnFocusToParentAction.Invoke();
                
                break;
        }
    }
}