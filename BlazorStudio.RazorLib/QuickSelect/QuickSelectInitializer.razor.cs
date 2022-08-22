using BlazorStudio.ClassLib.Store.QuickSelectCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.QuickSelect;

public partial class QuickSelectInitializer : FluxorComponent
{
    [Inject]
    private IState<QuickSelectState> QuickSelectStateWrap { get; set; } = null!;
}