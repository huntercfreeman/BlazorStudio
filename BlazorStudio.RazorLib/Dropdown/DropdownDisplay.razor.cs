using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Dropdown;

public partial class DropdownDisplay : FluxorComponent
{
    [Inject]
    private IState<DropdownState> DropdownStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    public Dimensions Dimensions { get; set; } = null!;
    [Parameter]
    public DropdownKey DropdownKey { get; set; } = null!;
    [Parameter]
    public bool CloseOnOutOfBoundsClick { get; set; } = true;
    [Parameter]
    public RenderFragment ChildContent { get; set; } = null!;

    private void ClickedOutOfBoundsOnClick(MouseEventArgs mouseEventArgs)
    {
        Dispatcher.Dispatch(new SetActiveDropdownKeyAction(null));
    }
}