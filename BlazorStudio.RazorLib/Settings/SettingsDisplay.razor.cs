using BlazorStudio.ClassLib.Store.ThemeCase;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using ThemeFacts = BlazorTextEditor.RazorLib.Store.ThemeCase.ThemeFacts;

namespace BlazorStudio.RazorLib.Settings;

public partial class SettingsDisplay : FluxorComponent
{
    [Inject]
    private IState<ThemeState> ThemeStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private void DispatchSetThemeStateAction(ThemeRecord themeRecord)
    {
        TextEditorService.SetTheme(themeRecord.ThemeColorKind == ThemeColorKind.Light
            ? ThemeFacts.BlazorTextEditorLight
            : ThemeFacts.BlazorTextEditorDark);

        Dispatcher.Dispatch(new SetThemeStateAction(themeRecord));
    }
}