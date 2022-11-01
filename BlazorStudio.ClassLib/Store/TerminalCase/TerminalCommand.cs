using System.Text;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record TerminalCommand(
    TerminalCommandKey TerminalCommandKey,
    string? WorkingDirectoryAbsoluteFilePathString,
    Func<TerminalCommand, Task> CommandFunc,
    StringBuilder StandardOut,
    StringBuilder StandardError);