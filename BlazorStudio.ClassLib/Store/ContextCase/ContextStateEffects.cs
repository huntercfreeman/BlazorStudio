using Fluxor;

namespace BlazorStudio.ClassLib.Store.ContextCase;

public class ContextStateEffects
{
    private readonly IState<ContextState> _contextStateWrap;

    public ContextStateEffects(IState<ContextState> contextStateWrap)
    {
        _contextStateWrap = contextStateWrap;
    }
    
    [EffectMethod]
    public async Task HandleKeymapEventAction(KeymapEventAction keymapEventAction,
        IDispatcher dispatcher)
    {
        var activeContextRecords = _contextStateWrap.Value.ActiveContextRecords;
        
        foreach (var contextRecord in activeContextRecords)
        {
            if (contextRecord.Keymap.Map.TryGetValue(keymapEventAction.KeyDownEventRecord, out var command))
            {
                if (command.Action is not null)
                {
                    command.Action.Invoke();
                    break;
                }
            }
        }
    }
}