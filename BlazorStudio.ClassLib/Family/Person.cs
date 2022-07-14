namespace BlazorStudio.ClassLib.Family;

public class Person
{
    public Person(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public PersonKey PersonKey { get; } = PersonKey.NewPersonKey();
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<Person> Children { get; set; } = new List<Person>();
    public string DisplayName => $"{FirstName} {LastName}";
}