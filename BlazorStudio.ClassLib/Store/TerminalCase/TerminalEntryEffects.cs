﻿using System.Collections.Concurrent;
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
            var process = new Process();

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
                if (enqueueProcessOnTerminalEntryAction.OnAnyDataReceived is not null)
                {
                    enqueueProcessOnTerminalEntryAction.OnAnyDataReceived(sender, e);
                }
            }

            process.OutputDataReceived += OutputDataReceived;

            try
            {
                enqueueProcessOnTerminalEntryAction.OnStart.Invoke();
                process.Start();

                process.BeginOutputReadLine();

                await process.WaitForExitAsync();
            }
            finally
            {
                process.CancelOutputRead();
                process.OutputDataReceived -= OutputDataReceived;
                enqueueProcessOnTerminalEntryAction.OnEnd.Invoke();
            }
        });
    }
}