using BlazorStudio.ClassLib.Store.FooterWindowCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class FooterWindowDisplay : FluxorComponent
{
    [Inject]
    private IState<FooterWindowState> FooterWindowStateWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;
}