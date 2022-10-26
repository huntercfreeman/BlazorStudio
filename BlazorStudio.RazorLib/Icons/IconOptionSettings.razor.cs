using BlazorStudio.ClassLib.Store.IconCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Icons;

public partial class IconOptionSettings : FluxorComponent
{
    [Inject]
    private IState<IconOptionsState> IconOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private int _iconSizeInPixels;

    private int IconSizeInPixels
    {
        get => _iconSizeInPixels;
        set
        {
            _iconSizeInPixels = value;
            Dispatcher.Dispatch(new SetIconOptionsStateAction(new DimensionUnit()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = _iconSizeInPixels,
            }));
        }
    }

    protected override void OnParametersSet()
    {
        _iconSizeInPixels = (int)IconOptionsStateWrap.Value.IconSize.Value;

        base.OnParametersSet();
    }
}