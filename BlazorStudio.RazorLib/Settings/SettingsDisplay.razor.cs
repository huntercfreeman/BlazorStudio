using BlazorStudio.ClassLib.Store.ThemeCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Settings;

public partial class SettingsDisplay : FluxorComponent
{
    [Inject]
    private IState<ThemeState> ThemeStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private void DispatchSetThemeStateAction(ThemeRecord themeRecord)
    {
        Dispatcher.Dispatch(new SetThemeStateAction(themeRecord));        
    }
}