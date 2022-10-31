using System.Text;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record TerminalCommand(
    TerminalCommandKey TerminalCommandKey,
    Func<TerminalSession, Task> CommandFunc,
    StringBuilder StandardOut,
    StringBuilder StandardError);