using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.ThemeCase;

public static partial class ThemeFacts
{
    public static partial class HighContrast
    {
        public static ImmutableArray<ThemeKey> AllHighContrastThemeKeys = Array.Empty<ThemeKey>()
            .Union(Dark.AllHighContrastDarkThemeKeys)
            .Union(Light.AllHighContrastLightThemeKeys)
            .ToImmutableArray();
    }
}