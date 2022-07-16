namespace BlazorStudio.ClassLib.TaskModelManager;

public interface IKey
{
    public Guid Id { get; }
    public string Title { get; }
}