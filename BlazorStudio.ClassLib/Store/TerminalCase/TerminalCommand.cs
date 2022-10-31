using System.Text;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record TerminalCommand(
    TerminalCommandKey TerminalCommandKey,
    Func<Task> CommandFunc,
    StringBuilder StandardOut,
    StringBuilder StandardError);