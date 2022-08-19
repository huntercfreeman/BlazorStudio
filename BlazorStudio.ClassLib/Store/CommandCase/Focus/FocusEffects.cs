using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.Store.ContextCase;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.CommandCase.Focus;

public class FocusEffects
{
    private readonly IState<ContextState> _contextStateWrap;

    public FocusEffects(IState<ContextState> contextStateWrap)
    {
        _contextStateWrap = contextStateWrap;
    }

    [EffectMethod(typeof(FocusFolderExplorerAction))]
    public async Task HandleFocusFolderExplorerAction(IDispatcher dispatcher)
    {
        _contextStateWrap.Value.ContextRecords[ContextFacts.FolderExplorerContext.ContextKey]
            .InvokeOnFocusRequestedEventHandler();
    }
    
    [EffectMethod(typeof(FocusMainLayoutAction))]
    public async Task HandleFocusMainLayoutAction(IDispatcher dispatcher)
    {
        _contextStateWrap.Value.ContextRecords[ContextFacts.GlobalContext.ContextKey]
            .InvokeOnFocusRequestedEventHandler();
    }
}