namespace BlazorStudio.ClassLib.Contexts;

public record ContextKey(Guid Guid)
{
    public static ContextKey NewContextKey()
    {
        return new ContextKey(Guid.NewGuid());
    }
}