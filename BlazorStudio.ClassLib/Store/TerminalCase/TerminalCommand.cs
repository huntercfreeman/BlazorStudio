using System.Text;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record TerminalCommand(
    TerminalCommandKey TerminalCommandKey,
    string Command);
