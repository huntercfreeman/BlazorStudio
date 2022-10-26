using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.ThemeCase;

public static partial class ThemeFacts
{
    public static partial class Default
    {
        public static class Light
        {
            public static ThemeKey BstudioDefaultLightTheme =
                ThemeKey.NewThemeKey("bstudio_default-light-theme");

            public static ImmutableArray<ThemeKey> AllDefaultLightThemeKeys = new ThemeKey[]
            {
                BstudioDefaultLightTheme,
            }.ToImmutableArray();
        }
    }
}