using BlazorStudio.ClassLib.Store.FontCase;
using BlazorStudio.ClassLib.Store.IconCase;
using BlazorStudio.ClassLib.Store.LocalStorageCase;
using BlazorStudio.ClassLib.Store.ThemeCase;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ThemeFacts = BlazorTextEditor.RazorLib.Store.ThemeCase.ThemeFacts;

namespace BlazorStudio.RazorLib.Settings;

public partial class SettingsDisplay : FluxorComponent
{
    [Inject]
    private IState<ThemeState> ThemeStateWrap { get; set; } = null!;
    [Inject]
    private IState<FontState> FontStateWrap { get; set; } = null!;
    [Inject]
    private IState<IconState> IconStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    private bool _globalShowNewlines;
    private bool _globalShowWhitespace;

    public bool GlobalShowNewlines
    {
        get => _globalShowNewlines;
        set => TextEditorService.SetShowNewlines(value);
    }

    public bool GlobalShowWhitespace
    {
        get => _globalShowWhitespace;
        set => TextEditorService.SetShowWhitespace(value);
    }

    protected override void OnInitialized()
    {
        TextEditorService.OnTextEditorStatesChanged += TextEditorServiceOnTextEditorStatesChanged;
        
        TextEditorServiceOnTextEditorStatesChanged();
        
        base.OnInitialized();
    }

    private void TextEditorServiceOnTextEditorStatesChanged()
    {
        _globalShowNewlines = TextEditorService.GlobalShowNewlines;
        _globalShowWhitespace = TextEditorService.GlobalShowWhitespace;
    }

    private void DispatchSetThemeStateAction(ThemeRecord themeRecord)
    {
        TextEditorService.SetTheme(themeRecord.ThemeColorKind == ThemeColorKind.Light
            ? ThemeFacts.BlazorTextEditorLight
            : ThemeFacts.BlazorTextEditorDark);

        Dispatcher.Dispatch(new SetThemeStateAction(themeRecord));
    }
    
    private Task PersistSettingsLocallyOnClick()
    {
        var fontSize = FontStateWrap.Value.FontSizeInPixels;
        var iconSize = IconStateWrap.Value.IconSizeInPixels;
        var themeClassCssString = ThemeStateWrap.Value.ActiveThemeRecord.ClassCssString;
        
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

    protected override void Dispose(bool disposing)
    {
        TextEditorService.OnTextEditorStatesChanged -= TextEditorServiceOnTextEditorStatesChanged;
        
        base.Dispose(disposing);
    }
}