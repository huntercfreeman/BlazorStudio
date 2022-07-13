using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.ThemeCase;

public static partial class ThemeFacts
{
    public static partial class Default
    {
        public static ImmutableArray<ThemeKey> AllDefaultThemeKeys = Array.Empty<ThemeKey>()
            .Union(Dark.AllDefaultDarkThemeKeys)
            .Union(Light.AllDefaultLightThemeKeys)
            .ToImmutableArray();
    }
}