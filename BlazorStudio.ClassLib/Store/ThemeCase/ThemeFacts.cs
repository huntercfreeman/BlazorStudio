using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.ThemeCase;

public static class ThemeFacts
{
    public static readonly ThemeRecord DarkTheme = new(
        "bte_dark-theme",
        ThemeColorKind.Dark,
        ThemeContrastKind.Default);
    
    public static readonly ThemeRecord LightTheme = new(
        "bte_light-theme",
        ThemeColorKind.Light,
        ThemeContrastKind.Default);
    
    public static readonly ImmutableArray<ThemeRecord> DefaultThemeRecords = new[]
    {
        DarkTheme,
        LightTheme
    }.ToImmutableArray();
}