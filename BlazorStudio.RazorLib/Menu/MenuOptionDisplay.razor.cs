using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.Store.MenuCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Menu;

public partial class MenuOptionDisplay : ComponentBase
{
    [Inject]
    private IState<DropdownState> DropdownStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    public MenuOptionRecord MenuOptionRecord { get; set; } = null!;

    private string HasSubmenuOpenCssClass => DropdownStateWrap.Value.ActiveDropdownKeys.Any(x => x == _dropdownKey)
        ? "bstudio_sub-menu-is-open"
        : string.Empty;

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

    private DropdownKey _dropdownKey = DropdownKey.NewDropdownKey();

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
}