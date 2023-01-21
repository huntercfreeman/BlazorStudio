using System.Collections.Immutable;
using BlazorALaCarte.Shared.Theme;

namespace BlazorStudio.ClassLib;

public class BlazorStudioTextEditorColorThemeFacts
{
    public static readonly ThemeRecord LightTheme = new ThemeRecord(
        ThemeKey.NewThemeKey(),
        "BlazorStudio Light Theme",
        "balc_light-theme",
        ThemeContrastKind.Default,
        ThemeColorKind.Light);

    public static readonly ThemeRecord DarkTheme = new ThemeRecord(
        ThemeKey.NewThemeKey(),
        "BlazorStudio Dark Theme",
        "balc_dark-theme",
        ThemeContrastKind.Default,
        ThemeColorKind.Dark);

    public static readonly ImmutableArray<ThemeRecord> BlazorStudioTextEditorThemes = new ThemeRecord[]
    {
        LightTheme,
        DarkTheme
    }.ToImmutableArray();
}