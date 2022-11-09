using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.Store.FooterCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class BlazorTextEditorFooter : FluxorComponent
{
    [Inject]
    private IState<FooterState> FooterStateWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions FooterElementDimensions { get; set; } = null!;
}