using Fluxor;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.ClassLib.Store.DragCase;

[FeatureState]
public record DragState(bool ShouldDisplay, MouseEventArgs? MouseEventArgs)
{
    public DragState() : this(false, null)
    {
        
    }
}

public record SetDragStateAction(bool ShouldDisplay, MouseEventArgs? MouseEventArgs);

public class DragStateReducer
{
    [ReducerMethod]
    public static DragState ReduceSetDragStateAction(DragState previousDragState,
        SetDragStateAction setDragStateAction)
    {
        return previousDragState with
        {
            ShouldDisplay = setDragStateAction.ShouldDisplay,
            MouseEventArgs = setDragStateAction.MouseEventArgs
        };
    }
}