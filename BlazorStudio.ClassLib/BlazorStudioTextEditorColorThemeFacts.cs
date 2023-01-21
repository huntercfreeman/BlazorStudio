using System.Collections.Immutable;
using BlazorALaCarte.Shared.Theme;

namespace BlazorStudio.ClassLib;

public class BlazorStudioTextEditorColorThemeFacts
{
    public static readonly ThemeRecord LightTheme = new ThemeRecord(
        new ThemeKey(Guid.Parse("8165209b-0cea-45b4-b6dd-e5661b319c73")),
        "BlazorStudio Light Theme",
        "balc_light-theme",
        ThemeContrastKind.Default,
        ThemeColorKind.Light);

    public static readonly ThemeRecord DarkTheme = new ThemeRecord(
        new ThemeKey(Guid.Parse("56d64327-03c2-48a3-b086-11b101826efb")),
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