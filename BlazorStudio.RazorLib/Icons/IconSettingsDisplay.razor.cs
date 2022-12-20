using BlazorALaCarte.Shared.IconCase;
using BlazorStudio.ClassLib.Store.FontCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Icons;

public partial class IconSettingsDisplay : FluxorComponent
{
    [Inject]
    private IState<IconState> IconStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private const int MINIMUM_ICON_SIZE_IN_PIXELS = 5;
    
    private int _iconSizeInPixels;

    private int IconSizeInPixels
    {
        get => _iconSizeInPixels;
        set
        {
            if (value < MINIMUM_ICON_SIZE_IN_PIXELS)
                _iconSizeInPixels = MINIMUM_ICON_SIZE_IN_PIXELS;
            else
                _iconSizeInPixels = value;
            
            Dispatcher.Dispatch(new SetIconSizeInPixelsAction(_iconSizeInPixels));
        }
    }

    protected override Task OnInitializedAsync()
    {
        _iconSizeInPixels = IconStateWrap.Value.IconSizeInPixels;
        
        return base.OnInitializedAsync();
    }
}