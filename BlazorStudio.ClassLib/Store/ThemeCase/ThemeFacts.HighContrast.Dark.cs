using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.ThemeCase;

public static partial class ThemeFacts
{
    public static partial class HighContrast
    {
        public static class Dark
        {
            public static ThemeKey BstudioHighContrastDarkTheme =
                ThemeKey.NewThemeKey("bstudio_default-dark-theme");

            public static ImmutableArray<ThemeKey> AllHighContrastDarkThemeKeys = new ThemeKey[]
            {
                BstudioHighContrastDarkTheme
            }.ToImmutableArray();
        }
    }
}