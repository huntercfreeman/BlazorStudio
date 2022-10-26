namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record TerminalEntry(TerminalEntryKey TerminalEntryKey,
    string Title,
    bool IsExecuting,
    Func<string, string> ParseOutputFunc,
    bool IsMarkupString);