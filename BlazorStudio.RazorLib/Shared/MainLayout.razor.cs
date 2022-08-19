using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.FontCase;
using BlazorStudio.ClassLib.Store.ThemeCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class MainLayout : FluxorLayout
{
    [Inject]
    private IState<ThemeState> ThemeStateWrap { get; set; } = null!;
    [Inject]
    private IState<FontOptionsState> FontOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IStateSelection<ContextState, ContextRecord> ContextStateSelector { get; set; } = null!;

    protected override void OnInitialized()
    {
        ContextStateSelector.Select(x => x.ContextRecords[ContextFacts.GlobalContext.ContextKey]);
        
        base.OnInitialized();
    }
}