using System.Collections.Immutable;
using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.MenuCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Menu;

public partial class MenuDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [CascadingParameter]
    public DropdownKey DropdownKey { get; set; } = null!;
    
    [Parameter]
    public IEnumerable<MenuOptionRecord> MenuOptionRecords { get; set; } = null!;
    [Parameter]
    public bool ShouldCategorizeByMenuOptionKind { get; set; }
    [Parameter]
    public ElementReference? FocusAfterTarget { get; set; }

    private MenuOptionRecord[] _cachedMenuOptionRecords = Array.Empty<MenuOptionRecord>();
    private ElementReference _menuDisplayElementReference;
    private int? _activeMenuOptionIndex = null;

    protected override void OnInitialized()
    {
        ReloadParameters();

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await _menuDisplayElementReference.FocusAsync();
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private void ReloadParameters()
    {
        _cachedMenuOptionRecords = MenuOptionRecords
            .ToArray();
    }
    
    public async Task ReloadParametersAsync()
    {
        ReloadParameters();

        await InvokeAsync(StateHasChanged);
    }
    
    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        var keyDownEventRecord = new KeyDownEventRecord(
            keyboardEventArgs.Key,
            keyboardEventArgs.Code,
            keyboardEventArgs.CtrlKey,
            keyboardEventArgs.ShiftKey,
            keyboardEventArgs.AltKey);
        
        if (KeyboardKeyFacts.IsMovementKey(keyDownEventRecord) ||
            KeyboardKeyFacts.IsAlternateMovementKey(keyDownEventRecord))
        {
            switch (keyDownEventRecord.Key)
            {
                case KeyboardKeyFacts.MovementKeys.ARROW_LEFT_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_LEFT_KEY:
                    // TODO: Close submenu
                    break;
                case KeyboardKeyFacts.MovementKeys.ARROW_DOWN_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_DOWN_KEY:
                    if (_activeMenuOptionIndex is null &&
                        _cachedMenuOptionRecords.Length > 0)
                    {
                        _activeMenuOptionIndex = 0;
                    }
                    else if (_activeMenuOptionIndex < _cachedMenuOptionRecords.Length - 1)
                    {
                        _activeMenuOptionIndex++;
                    }
                    else if (_cachedMenuOptionRecords.Length > 0)
                    {
                        // Wrap around
                        _activeMenuOptionIndex = 0;
                    }
                    break;
                case KeyboardKeyFacts.MovementKeys.ARROW_UP_KEY:
                case KeyboardKeyFacts.AlternateMovementKeys.ARROW_UP_KEY:
                    if (_activeMenuOptionIndex is null &&
                        _cachedMenuOptionRecords.Length > 0)
                    {
                        _activeMenuOptionIndex = _cachedMenuOptionRecords.Length - 1;
                    }
                    else if (_activeMenuOptionIndex > 0)
                    {
                        _activeMenuOptionIndex--;
                    }
                    else if (_cachedMenuOptionRecords.Length > 0)
                    {
                        // Wrap around
                        _activeMenuOptionIndex = _cachedMenuOptionRecords.Length - 1;
                    }
                    break;
                case KeyboardKeyFacts.MovementKeys.HOME_KEY:
                    if (_cachedMenuOptionRecords.Length > 0)
                    {
                        _activeMenuOptionIndex = 0;
                    }
                    break;
                case KeyboardKeyFacts.MovementKeys.END_KEY:
                    if (_cachedMenuOptionRecords.Length > 0)
                    {
                        _activeMenuOptionIndex = _cachedMenuOptionRecords.Length - 1;
                    }
                    break;
            }
        }
        else if (KeyboardKeyFacts.IsMetaKey(keyDownEventRecord))
        {
            switch (keyDownEventRecord.Key)
            {
                case KeyboardKeyFacts.MetaKeys.ESCAPE_KEY:
                    Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());

                    if (FocusAfterTarget is not null)
                    {
                        await FocusAfterTarget.Value.FocusAsync();
                    }
                    break;                    
            }
        }
    }
}