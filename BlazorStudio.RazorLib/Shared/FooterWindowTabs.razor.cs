using BlazorStudio.ClassLib.Store.FooterWindowCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class FooterWindowTabs : FluxorComponent
{
    [Inject]
    private IState<FooterWindowState> FooterWindowStateWrap { get; set; } = null!;
}