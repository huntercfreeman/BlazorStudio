using System.Diagnostics;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

/// <summary>
/// Do not mix OnAnyDataReceivedAsync and OnAnyDataReceived it is one or the other
/// </summary>
public record EnqueueProcessOnTerminalEntryAction(TerminalEntryKey TerminalEntryKey,
    string Command,
    Action OnStart,
    Action<Process> OnEnd,
    Action<object, DataReceivedEventArgs>? OnAnyDataReceivedAsync,
    Action<string>? OnAnyDataReceived,
    CancellationToken CancellationToken);

public record SetTerminalEntryIsExecutingAction(TerminalEntryKey TerminalEntryKey,
    bool IsExecuting);