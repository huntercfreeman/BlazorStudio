using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Fluxor;
using System.Collections.Concurrent;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public class TerminalStateEffects
{
    private readonly Dictionary<TerminalEntryKey, TerminalEntryEffects> _terminalEffects = new();

    private readonly IState<TerminalState> _terminalStateWrap;
    private readonly IState<TerminalSettingsState> _terminalSettingsStateWrap;
    private readonly ConcurrentQueue<Func<Task>> _handleEffectQueue = new();
    private readonly SemaphoreSlim _executeHandleEffectSemaphoreSlim = new(1, 1);

    public TerminalStateEffects(IState<TerminalState> terminalStateWrap,
        IState<TerminalSettingsState> terminalSettingsStateWrap)
    {
        _terminalStateWrap = terminalStateWrap;
        _terminalSettingsStateWrap = terminalSettingsStateWrap;

        var localTerminalStateWrap = _terminalStateWrap.Value;

        foreach (var terminalEntry in localTerminalStateWrap.TerminalEntries)
        {
            _terminalEffects.Add(terminalEntry.TerminalEntryKey, 
                new TerminalEntryEffects(terminalStateWrap, _terminalSettingsStateWrap, terminalEntry));
        }
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

    [EffectMethod]
    public async Task HandleEnqueueProcessOnTerminalEntryAction(EnqueueProcessOnTerminalEntryAction enqueueProcessOnTerminalEntryAction,
        IDispatcher dispatcher)
    {
        await QueueHandleEffectAsync(async () =>
        {
            var target = _terminalEffects[enqueueProcessOnTerminalEntryAction.TerminalEntryKey];

            await target.HandleEnqueueProcessOnTerminalEntryAction(enqueueProcessOnTerminalEntryAction, dispatcher);
        });
    }
}