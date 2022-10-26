using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.DragCase;
using BlazorStudio.ClassLib.Store.FontCase;
using BlazorStudio.ClassLib.Store.ThemeCase;
using BlazorStudio.RazorLib.ContextCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Shared;

public partial class MainLayout : FluxorLayout
{
    [Inject]
    private IState<ThemeState> ThemeStateWrap { get; set; } = null!;
    [Inject]
    private IState<FontOptionsState> FontOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;

    private string CssClassString =>
        $"bstudio_main-layout bstudio_focus-display-extra-large {UnselectableCssClassString}";

    private string UnselectableCssClassString => DragStateWrap.Value.IsDisplayed
        ? "pte_unselectable"
        : string.Empty;
}