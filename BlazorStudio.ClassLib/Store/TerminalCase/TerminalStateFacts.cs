namespace BlazorStudio.ClassLib.Store.TerminalCase;

public static class TerminalStateFacts
{
    public static readonly TerminalEntry ProgramTerminalEntry = new(TerminalEntryKey.NewTerminalEntryKey(), "Program", false, null);
    public static readonly TerminalEntry GeneralTerminalEntry = new(TerminalEntryKey.NewTerminalEntryKey(), "General", false, null);
}