using System.Collections.Immutable;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public static class TerminalCommandFacts
{
    public static readonly TerminalCommandKey EXECUTION_TERMINAL = 
        TerminalCommandKey.NewTerminalCommandKey("Execution");
    
    public static readonly TerminalCommandKey GENERAL_TERMINAL = 
        TerminalCommandKey.NewTerminalCommandKey("General");

    public static readonly ImmutableArray<TerminalCommandKey> TERMINAL_COMMAND_KEYS = new[]
    {
        EXECUTION_TERMINAL,
        GENERAL_TERMINAL
    }.ToImmutableArray();
}