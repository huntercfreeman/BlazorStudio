namespace BlazorStudio.ClassLib.Store.DropdownCase;

public record DropdownKey(Guid Guid)
{
    public static DropdownKey NewDropdownKey()
    {
        return new DropdownKey(Guid.NewGuid());
    }
}