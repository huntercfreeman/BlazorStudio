using BlazorStudio.ClassLib.Store.FontCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Font;

public partial class FontOptionSettings : FluxorComponent
{
    private int _iconSizeInPixels;
    [Inject]
    private IState<FontOptionsState> FontOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private int FontSizeInPixels
    {
        get => _iconSizeInPixels;
        set
        {
            _iconSizeInPixels = value;
            Dispatcher.Dispatch(new SetFontOptionsStateAction(new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = _iconSizeInPixels,
            }));
        }
    }

    protected override void OnParametersSet()
    {
        _iconSizeInPixels = (int)FontOptionsStateWrap.Value.FontSize.Value;

        base.OnParametersSet();
    }
}