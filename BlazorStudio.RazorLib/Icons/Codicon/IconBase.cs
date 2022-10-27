using BlazorStudio.ClassLib.Store.IconCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Icons.Codicon;

public class IconBase : FluxorComponent
{
    [Inject]
    private IState<IconState> IconStateWrap { get; set; } = null!;

    [Parameter]
    public int? WidthInPixelsOverride { get; set; }
    [Parameter]
    public int? HeightInPixelsOverride { get; set; }
    
    protected int WidthInPixels => WidthInPixelsOverride ?? IconStateWrap.Value.IconSizeInPixels;
    protected int HeightInPixels => HeightInPixelsOverride ?? IconStateWrap.Value.IconSizeInPixels;
}