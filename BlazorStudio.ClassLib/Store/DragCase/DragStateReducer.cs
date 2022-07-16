using Fluxor;

namespace BlazorStudio.ClassLib.Store.DragCase;

public class DragStateReducer
{
    [ReducerMethod]
    public DragState ReduceSetDragStateAction(DragState previousDragState,
        SetDragStateAction setDragStateAction)
    {
        return new DragState(setDragStateAction.IsDisplayed, setDragStateAction.MouseEventArgs);
    }
}