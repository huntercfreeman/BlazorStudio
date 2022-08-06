namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record SetTerminalEntryOutputAction(TerminalEntryKey TerminalEntryKey,
    string? Output);