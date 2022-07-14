namespace BlazorStudio.ClassLib.Family;

public record Person(PersonKey PersonKey,
    string FirstName,
    string LastName)
{
    public string DisplayName => $"{FirstName} {LastName}";
}

public record PersonKey(Guid Guid)
{
    public static PersonKey NewPersonKey()
    {
        return new PersonKey(Guid.NewGuid());
    }
}