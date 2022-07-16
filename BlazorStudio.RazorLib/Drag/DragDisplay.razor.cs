using BlazorStudio.ClassLib.Mouse;
using BlazorStudio.ClassLib.Store.DragCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Drag;

public partial class DragDisplay : FluxorComponent
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private string StyleCss => DragStateWrap.Value.IsDisplayed
        ? string.Empty
        : "display: none;";

    private SetDragStateAction ConstructClearDragStateAction() => 
        new SetDragStateAction(false, new MouseEventArgs());

    private bool _previousIsDisplayed;
    private ElementReference _dragDisplayElement;

    protected override void OnAfterRender(bool firstRender)
    {
        var dragState = DragStateWrap.Value;

        if (_previousIsDisplayed != dragState.IsDisplayed)
        {
            _previousIsDisplayed = dragState.IsDisplayed;

            if (dragState.IsDisplayed)
            {
                _dragDisplayElement.FocusAsync();
            }
        }

        base.OnAfterRender(firstRender);
    }

    private void DispatchNotifyMouseEventOnMouseMove(MouseEventArgs mouseEventArgs)
    {
        if (mouseEventArgs.Button != MouseFacts.MOUSE_LEFT_BUTTON)
        {
            Dispatcher.Dispatch(ConstructClearDragStateAction());
        }
        else
        {
            Dispatcher.Dispatch(new SetDragStateAction(true, mouseEventArgs));
        }
    }
    
    private void DispatchHideOnMouseUp()
    {
        Dispatcher.Dispatch(ConstructClearDragStateAction());
    }
    
    private void DispatchHideOnFocusOut()
    {
        Dispatcher.Dispatch(ConstructClearDragStateAction());
    }
}