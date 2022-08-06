using System.Collections.Concurrent;
using System.Diagnostics;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public class TerminalEntryEffects
{
    private readonly IState<TerminalState> _terminalStateWrap;
    private readonly TerminalEntry _terminalEntry;
    private readonly ConcurrentQueue<Func<Task>> _handleEffectQueue = new();
    private readonly SemaphoreSlim _executeHandleEffectSemaphoreSlim = new(1, 1);

    public TerminalEntryEffects(IState<TerminalState> terminalStateWrap, TerminalEntry terminalEntry)
    {
        _terminalStateWrap = terminalStateWrap;
        _terminalEntry = terminalEntry;
    }

    private async Task QueueHandleEffectAsync(Func<Task> func)
    {
        _handleEffectQueue.Enqueue(func);

        try
        {
            await _executeHandleEffectSemaphoreSlim.WaitAsync();

            if (_handleEffectQueue.TryDequeue(out var fifoHandleEffect))
            {
                await fifoHandleEffect!.Invoke();
            }
        }
        finally
        {
            _executeHandleEffectSemaphoreSlim.Release();
        }
    }

    public async Task HandleEnqueueProcessOnTerminalEntryAction(EnqueueProcessOnTerminalEntryAction enqueueProcessOnTerminalEntryAction,
        IDispatcher dispatcher)
    {
        await QueueHandleEffectAsync(async () =>
        {
            if (enqueueProcessOnTerminalEntryAction.CancellationToken.IsCancellationRequested)
            {
                if (enqueueProcessOnTerminalEntryAction.OnCancelled is not null)
                {
                    enqueueProcessOnTerminalEntryAction.OnCancelled.Invoke();
                }

                return;
            }

            var process = new Process();

            if (enqueueProcessOnTerminalEntryAction.WorkingDirectoryAbsoluteFilePath is not null)
            {
                process.StartInfo.WorkingDirectory = enqueueProcessOnTerminalEntryAction.WorkingDirectoryAbsoluteFilePath
                    .GetAbsoluteFilePathString();
            }

            // Start the child process.
            process.StartInfo.FileName = "cmd.exe";
            // 2>&1 combines stdout and stderr
            process.StartInfo.Arguments = $"/c {enqueueProcessOnTerminalEntryAction.Command} 2>&1";
            // Redirect the output stream of the child process.
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            void OutputDataReceived(object sender, DataReceivedEventArgs e)
            {
                dispatcher.Dispatch(new SetTerminalEntryOutputAction(_terminalEntry.TerminalEntryKey, e.Data));

                if (enqueueProcessOnTerminalEntryAction.OnAnyDataReceivedAsync != null)
                    enqueueProcessOnTerminalEntryAction.OnAnyDataReceivedAsync(sender, e);
            }

            if (enqueueProcessOnTerminalEntryAction.OnAnyDataReceivedAsync is not null)
            {
                process.OutputDataReceived += OutputDataReceived;
            }

            try
            {
                dispatcher.Dispatch(new SetTerminalEntryIsExecutingAction(_terminalEntry.TerminalEntryKey,
                    true));

                enqueueProcessOnTerminalEntryAction.OnStart.Invoke();
                process.Start();

                if (enqueueProcessOnTerminalEntryAction.OnAnyDataReceivedAsync is not null)
                {
                    process.BeginOutputReadLine();
                }
                else if (enqueueProcessOnTerminalEntryAction.OnAnyDataReceived is not null)
                {
                    var output = await process.StandardOutput.ReadToEndAsync();

                    dispatcher.Dispatch(
                        new SetTerminalEntryOutputAction(_terminalEntry.TerminalEntryKey, output));

                    enqueueProcessOnTerminalEntryAction.OnAnyDataReceived
                        .Invoke(output);
                }
                else
                {
                    var output = await process.StandardOutput.ReadToEndAsync();

                    dispatcher.Dispatch(
                        new SetTerminalEntryOutputAction(_terminalEntry.TerminalEntryKey, output));
                }

                await process.WaitForExitAsync();
            }
            finally
            {
                if (enqueueProcessOnTerminalEntryAction.OnAnyDataReceivedAsync is not null)
                {
                    process.CancelOutputRead();
                    process.OutputDataReceived -= OutputDataReceived;
                }

                enqueueProcessOnTerminalEntryAction.OnEnd.Invoke(process);

                dispatcher.Dispatch(new SetTerminalEntryIsExecutingAction(_terminalEntry.TerminalEntryKey,
                    false));
            }
        });
    }
}