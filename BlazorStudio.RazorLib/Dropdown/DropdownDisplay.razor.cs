using BlazorStudio.ClassLib.Store.DropdownCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Dropdown;

public partial class DropdownDisplay : FluxorComponent, IDisposable
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
    public DropdownKind DropdownKind { get; set; }
    [Parameter]
    public RenderFragment ChildContent { get; set; } = null!;

    private string DropdownKindCssString => DropdownKind switch
    {
        DropdownKind.Vertical => "position: relative;",
        DropdownKind.Horizontal => "position: relative; display: inline;",
        DropdownKind.Unset => string.Empty,
        _ => throw new ApplicationException($"The {nameof(DropdownKind)}: '{DropdownKind}' was not recognized."),
    };

    private void ClickedOutOfBoundsOnClick(MouseEventArgs mouseEventArgs)
    {
        Dispatcher.Dispatch(new ClearActiveDropdownKeysAction());
    }

    protected override void Dispose(bool disposing)
    {
        Dispatcher.Dispatch(new RemoveActiveDropdownKeyAction(DropdownKey));

        base.Dispose(disposing);
    }
}