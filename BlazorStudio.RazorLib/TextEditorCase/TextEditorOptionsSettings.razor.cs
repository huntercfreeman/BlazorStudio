using BlazorStudio.ClassLib.Store.IconCase;
using BlazorStudio.ClassLib.Store.TextEditorCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.TextEditorCase;

public partial class TextEditorOptionsSettings : FluxorComponent
{
    [Inject] 
    private IState<TextEditorOptionsState> TextEditorOptionsStateWrap { get; set; } = null!;
    [Inject] 
    private IDispatcher Dispatcher { get; set; } = null!;

    private int _fontSizeInPixels;

    private int FontSizeInPixels
    {
        get => _fontSizeInPixels;
        set
        {
            _fontSizeInPixels = value;
            
            Dispatcher.Dispatch(new SetTextEditorOptionsAction(new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = _fontSizeInPixels
            }));
        }
    }

    protected override void OnParametersSet()
    {
        _fontSizeInPixels = (int) TextEditorOptionsStateWrap.Value.FontSize.Value;

        base.OnParametersSet();
    }
}