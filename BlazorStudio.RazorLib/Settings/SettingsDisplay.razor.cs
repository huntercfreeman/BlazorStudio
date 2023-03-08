using BlazorCommon.RazorLib.Options;
using BlazorCommon.RazorLib.Store.ThemeCase;
using BlazorCommon.RazorLib.Theme;
using BlazorStudio.ClassLib.Store.FontCase;
using BlazorStudio.ClassLib.Store.LocalStorageCase;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Settings;

public partial class SettingsDisplay : FluxorComponent
{
    [Inject]
    private IState<ThemeRecordsCollection> ThemeRecordsCollectionWrap { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IState<FontState> FontStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private void DispatchSetThemeStateAction(ThemeRecord themeRecord)
    {
        TextEditorService.GlobalOptionsSetTheme(themeRecord.ThemeColorKind == ThemeColorKind.Light
            ? ThemeFacts.VisualStudioLightThemeClone
            : ThemeFacts.VisualStudioDarkThemeClone);

        AppOptionsService.SetActiveThemeRecordKey(themeRecord.ThemeKey);
    }
    
    private Task PersistSettingsLocallyOnClick()
    {
        var fontSize = FontStateWrap.Value.FontSizeInPixels;
        var iconSize = AppOptionsService.AppOptionsStateWrap.Value.Options.IconSizeInPixels;
        var themeClassCssString = AppOptionsService.GlobalThemeCssClassString;
        
        Dispatcher.Dispatch(new LocalStorageEffects.LocalStorageSetItemAction(
            "bstudio_fontSize",
            fontSize.ToString()));
        
        Dispatcher.Dispatch(new LocalStorageEffects.LocalStorageSetItemAction(
            "bstudio_iconSize",
            iconSize.ToString()));
        
        Dispatcher.Dispatch(new LocalStorageEffects.LocalStorageSetItemAction(
            "bstudio_themeClassCssString",
            themeClassCssString));

        return Task.CompletedTask;
    }
}