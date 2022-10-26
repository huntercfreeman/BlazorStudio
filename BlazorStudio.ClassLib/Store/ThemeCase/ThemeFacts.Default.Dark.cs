using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.ThemeCase;

public static partial class ThemeFacts
{
    public static partial class Default
    {
        public static class Dark
        {
            public static ThemeKey BstudioDefaultDarkTheme =
                ThemeKey.NewThemeKey("bstudio_default-dark-theme");

            public static ThemeKey BstudioDefaultDarkSolarizedTheme =
                ThemeKey.NewThemeKey("bstudio_default-dark-solarized-theme");

            public static ImmutableArray<ThemeKey> AllDefaultDarkThemeKeys = new[]
            {
                BstudioDefaultDarkTheme,
                BstudioDefaultDarkSolarizedTheme,
            }.ToImmutableArray();
        }
    }
}