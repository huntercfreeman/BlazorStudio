namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record TerminalCommandKey(Guid Guid)
{
    public static TerminalCommandKey NewTerminalCommandKey()
    {
        return new(Guid.NewGuid());
    }
}