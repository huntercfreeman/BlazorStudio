namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record TerminalCommandKey(Guid Guid, string? DisplayName)
{
    public static TerminalCommandKey NewTerminalCommandKey(string? displayName = null)
    {
        return new(Guid.NewGuid(), displayName);
    }
}