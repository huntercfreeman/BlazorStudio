using Fluxor;

namespace BlazorStudio.ClassLib.Store.TransformableCase;

public class ReduceTransformableOptions
{
    [ReducerMethod]
    public static TransformableOptionsState ReduceSetTransformableOptionsAction(
        TransformableOptionsState previousTransformableOptionsState,
        SetTransformableOptionsAction setTransformableOptionsAction)
    {
        return new TransformableOptionsState(setTransformableOptionsAction.ResizeHandleDimensionUnit);
    }
}