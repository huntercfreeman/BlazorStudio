namespace BlazorStudio.ClassLib.Store.TerminalCase;

public static class TerminalStateFacts
{
    public static readonly TerminalEntry ExecutionTerminalEntry = new(TerminalEntryKey.NewTerminalEntryKey(), "Execution");
    public static readonly TerminalEntry GeneralTerminalEntry = new(TerminalEntryKey.NewTerminalEntryKey(), "General");
}