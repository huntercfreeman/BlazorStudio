using BlazorStudio.ClassLib.Menu;
using BlazorStudio.ClassLib.Store.DropdownCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

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
    public MenuOptionRecord? ActiveMenuOptionRecord { get; set; }
    
    private readonly DropdownKey _subMenuDropdownKey = DropdownKey.NewDropdownKey();

    private string IsActiveCssClass =>
        (ActiveMenuOptionRecord?.Id ?? Guid.Empty) == MenuOptionRecord.Id
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
}