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

    protected override void Dispose(bool disposing)
    {
        TextEditorService.OnTextEditorStatesChanged -= TextEditorServiceOnTextEditorStatesChanged;
        
        base.Dispose(disposing);
    }
}