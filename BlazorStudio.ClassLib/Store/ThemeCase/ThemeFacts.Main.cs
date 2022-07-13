using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.ThemeCase;

public static partial class ThemeFacts
{
    public static ImmutableArray<ThemeKey> AllDefaultThemeKeys = Array.Empty<ThemeKey>()
        .Union(Default.AllDefaultThemeKeys)
        .Union(HighContrast.AllHighContrastThemeKeys)
        .ToImmutableArray();
}