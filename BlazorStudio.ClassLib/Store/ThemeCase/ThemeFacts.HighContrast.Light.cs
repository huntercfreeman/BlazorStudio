using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.ThemeCase;

public static partial class ThemeFacts
{
    public static partial class HighContrast
    {
        public static class Light
        {
            public static ThemeKey BstudioHighContrastLightTheme =
                ThemeKey.NewThemeKey("bstudio_default-light-theme");

            public static ImmutableArray<ThemeKey> AllHighContrastLightThemeKeys = new ThemeKey[]
            {
                BstudioHighContrastLightTheme
            }.ToImmutableArray();
        }
    }
}