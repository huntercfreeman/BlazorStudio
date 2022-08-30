using BlazorStudio.ClassLib.Keyboard;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Menu;

public partial class MenuOptionDisplay : FluxorComponent
{
    [Inject]
    private IState<DropdownState> DropdownStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter(Name = "ActiveMenuOptionIndex")]
    public int? ActiveMenuOptionIndex { get; set; }
    [CascadingParameter(Name = "CloseParentSubmenuFuncAsync")]
    public Func<Task>? CloseParentSubmenuFuncAsync { get; set; }

    [Parameter]
    public MenuOptionRecord MenuOptionRecord { get; set; } = null!;
    [Parameter]
    public int MenuOptionIndex { get; set; }

    private string HasSubmenuOpenCssClass => DropdownStateWrap.Value.ActiveDropdownKeys.Any(x => x == _dropdownKey)
        ? "bstudio_sub-menu-is-open"
        : string.Empty;

    private bool _displayWidget;
    private ElementReference _menuOptionDisplayElementReference;

    private Dimensions _dropdownDimensions = new()
    {
        DimensionsPositionKind = DimensionsPositionKind.Absolute,
        LeftCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = 0
            }
        },
        TopCalc = new List<DimensionUnit>
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = 0
            }
        },
    };

    private int _activeMenuOptionIndex;
    private bool _hasFocus;

    private DropdownKey _dropdownKey = DropdownKey.NewDropdownKey();
    
    protected override async Task OnParametersSetAsync()
    {
        if (ActiveMenuOptionIndex == MenuOptionIndex &&
            !_hasFocus)
        {
            await _menuOptionDisplayElementReference.FocusAsync();
        }
        
        await base.OnParametersSetAsync();
    }

    private void DispatchToggleActiveDropdownKeyActionOnClick(DropdownKey fileDropdownKey)
    {
        if (MenuOptionRecord.OnClickAction is not null)
        {
            MenuOptionRecord.OnClickAction();

            Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());

            return;
        }

        if (DropdownStateWrap.Value.ActiveDropdownKeys.Any(x => x == _dropdownKey))
            Dispatcher.Dispatch(new RemoveActiveDropdownKeyAction(fileDropdownKey));
        else
            Dispatcher.Dispatch(new AddActiveDropdownKeyAction(fileDropdownKey));
    }
    
    private async Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        var keyDownEventRecord = new KeyDownEventRecord(
            keyboardEventArgs.Key,
            keyboardEventArgs.Code,
            keyboardEventArgs.CtrlKey,
            keyboardEventArgs.ShiftKey,
            keyboardEventArgs.AltKey);
        
        switch (keyDownEventRecord.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
            {
                DispatchToggleActiveDropdownKeyActionOnClick(_dropdownKey);

                if (MenuOptionRecord.WidgetType is not null)
                    _displayWidget = !_displayWidget;

                break;
            }
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_LEFT:
                if (CloseParentSubmenuFuncAsync is not null)
                    await CloseParentSubmenuFuncAsync.Invoke();
                
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_RIGHT:
            {
                if (MenuOptionRecord.Children.Any())
                    DispatchToggleActiveDropdownKeyActionOnClick(_dropdownKey);
                
                break;
            }
        }
    }

    private async Task CloseSelfSubmenuAsync()
    {
        DispatchToggleActiveDropdownKeyActionOnClick(_dropdownKey);

        await _menuOptionDisplayElementReference.FocusAsync();
    }
}