namespace BlazorStudio.ClassLib.TaskModelManager;

public record TaskModelKey(Guid Guid, string Title)
{
    public static TaskModelKey NewTaskModelKey(string title)
    {
        return new TaskModelKey(Guid.NewGuid(), title);
    }
}