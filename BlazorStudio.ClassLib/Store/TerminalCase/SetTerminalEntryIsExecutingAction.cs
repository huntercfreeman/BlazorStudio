namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record SetTerminalEntryIsExecutingAction(TerminalEntryKey TerminalEntryKey,
    bool IsExecuting);