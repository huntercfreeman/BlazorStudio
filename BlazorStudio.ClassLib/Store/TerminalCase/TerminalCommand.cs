using System.Text;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record TerminalCommand(
    TerminalCommandKey TerminalCommandKey,
    Func<TerminalCommand, Task> CommandFunc);
