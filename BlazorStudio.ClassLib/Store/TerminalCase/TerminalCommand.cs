namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record TerminalCommand(
    TerminalCommandKey TerminalCommandKey,
    string Command,
    string? ChangeWorkingDirectoryTo = null,
    CancellationToken CancellationToken = default);
