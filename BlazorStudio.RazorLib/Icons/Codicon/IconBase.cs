using BlazorStudio.ClassLib.Store.IconCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Icons.Codicon;

public class IconBase : FluxorComponent
{
    [Inject] 
    private IState<IconOptionsState> IconOptionsStateWrap { get; set; } = null!;

    protected int WidthInPixels => (int)(WidthInPixelsOverride ?? IconOptionsStateWrap.Value.IconSize.Value);
    protected int HeightInPixels => (int)(HeightInPixelsOverride ?? IconOptionsStateWrap.Value.IconSize.Value);
    
    [Parameter]
    public int? WidthInPixelsOverride { get; set; }
    [Parameter]
    public int? HeightInPixelsOverride { get; set; }
}