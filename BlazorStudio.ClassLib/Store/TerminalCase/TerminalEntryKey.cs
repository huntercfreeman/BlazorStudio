namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record TerminalEntryKey(Guid Guid)
{
    public static TerminalEntryKey NewTerminalEntryKey()
    {
        return new TerminalEntryKey(Guid.NewGuid());
    }
}