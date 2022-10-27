using BlazorStudio.ClassLib.Store.FontCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.FontSettings;

public partial class FontSettingsDisplay : FluxorComponent
{
    [Inject]
    private IState<FontState> FontStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private int MINIMUM_FONT_SIZE_IN_PIXELS = 5;
    
    private int _fontSizeInPixels;

    private int FontSizeInPixels
    {
        get => _fontSizeInPixels;
        set
        {
            if (value < MINIMUM_FONT_SIZE_IN_PIXELS)
                _fontSizeInPixels = MINIMUM_FONT_SIZE_IN_PIXELS;
            else
                _fontSizeInPixels = value;
            
            Dispatcher.Dispatch(new SetFontSizeInPixelsAction(_fontSizeInPixels));
        }
    }

    protected override Task OnInitializedAsync()
    {
        _fontSizeInPixels = FontStateWrap.Value.FontSizeInPixels;
        
        return base.OnInitializedAsync();
    }
}