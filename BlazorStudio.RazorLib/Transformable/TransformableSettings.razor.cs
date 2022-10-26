using BlazorStudio.ClassLib.Store.TransformableCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Transformable;

public partial class TransformableSettings : FluxorComponent
{
    private int _resizeHandleSizeInPixels;
    [Inject]
    private IState<TransformableOptionsState> TransformableOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private int ResizeHandleSizeInPixels
    {
        get => _resizeHandleSizeInPixels;
        set
        {
            _resizeHandleSizeInPixels = value;
            Dispatcher.Dispatch(new SetTransformableOptionsAction(new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = _resizeHandleSizeInPixels,
            }));
        }
    }

    protected override void OnParametersSet()
    {
        _resizeHandleSizeInPixels = (int)TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value;

        base.OnParametersSet();
    }
}