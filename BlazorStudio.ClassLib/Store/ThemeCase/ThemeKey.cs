namespace BlazorStudio.ClassLib.Store.ThemeCase;

public record ThemeKey(Guid Guid, string KeyName)
{
    public static ThemeKey NewThemeKey(string keyName)
    {
        return new ThemeKey(Guid.NewGuid(), keyName);
    }
}