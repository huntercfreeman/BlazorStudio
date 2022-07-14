namespace BlazorStudio.ClassLib.Family;

public record PersonKey(Guid Guid)
{
    public static PersonKey NewPersonKey()
    {
        return new PersonKey(Guid.NewGuid());
    }
}