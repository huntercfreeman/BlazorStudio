namespace BlazorStudio.ClassLib.Commands;

public record CommandKey(Guid Guid)
{
    public static CommandKey NewCommandKey()
    {
        return new CommandKey(Guid.NewGuid());
    }
}