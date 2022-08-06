using System.Diagnostics;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public record EnqueueProcessOnTerminalEntryAction(TerminalEntryKey TerminalEntryKey,
    string Command,
    Action OnStart,
    Action<Process> OnEnd,
    Action<object, DataReceivedEventArgs>? OnAnyDataReceived,
    CancellationToken CancellationToken);