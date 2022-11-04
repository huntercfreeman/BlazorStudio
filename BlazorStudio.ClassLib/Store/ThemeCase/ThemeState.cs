using Fluxor;

namespace BlazorStudio.ClassLib.Store.ThemeCase;

[FeatureState]
public record ThemeState(ThemeRecord ActiveThemeRecord)
{
    public ThemeState() : this(ThemeFacts.LightTheme)
    {
        
    }
}