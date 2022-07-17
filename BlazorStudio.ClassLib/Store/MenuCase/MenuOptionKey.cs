namespace BlazorStudio.ClassLib.Store.MenuCase;

public record MenuOptionKey(Guid Guid)
{
    public static MenuOptionKey NewMenuOptionKey()
    {
        return new MenuOptionKey(Guid.NewGuid());
    }
}