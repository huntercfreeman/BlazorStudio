using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorTextEditor.RazorLib.Keyboard;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Menu;

public partial class MenuOptionDisplay : ComponentBase
{
    [Inject]
    private IState<DropdownStates> DropdownStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public MenuOptionRecord MenuOptionRecord { get; set; } = null!;
    [Parameter, EditorRequired]
    public int Index { get; set; }
    [Parameter, EditorRequired]
    public int ActiveMenuOptionRecordIndex { get; set; }
    
    private readonly DropdownKey _subMenuDropdownKey = DropdownKey.NewDropdownKey();

    private bool IsActive => Index == ActiveMenuOptionRecordIndex;
    
    private string IsActiveCssClass => IsActive
        ? "bstudio_active"
        : string.Empty;
    
    private string HasSubmenuActiveCssClass =>
        DropdownStatesWrap.Value.ActiveDropdownKeys.Any(x => x.Guid == _subMenuDropdownKey.Guid)
            ? "bstudio_active"
            : string.Empty;
    
    private void HandleOnClick()
    {
        if (MenuOptionRecord.OnClick is not null)
            MenuOptionRecord.OnClick.Invoke();
        
        if (MenuOptionRecord.SubMenu is not null)
            Dispatcher.Dispatch(new AddActiveDropdownKeyAction(_subMenuDropdownKey));
    }

    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        switch (keyboardEventArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_RIGHT:
                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
            case KeyboardKeyFacts.AlternateMovementKeys.ARROW_LEFT:
                break;
        }
    }
}