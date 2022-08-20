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

    [EffectMethod(typeof(FocusMainLayoutAction))]
    public async Task HandleFocusMainLayoutAction(IDispatcher dispatcher)
    {
        _contextStateWrap.Value.ContextRecords[ContextFacts.GlobalContext.ContextKey]
            .InvokeOnFocusRequestedEventHandler();
    }
    
    [EffectMethod(typeof(FocusFolderExplorerAction))]
    public async Task HandleFocusFolderExplorerAction(IDispatcher dispatcher)
    {
        _contextStateWrap.Value.ContextRecords[ContextFacts.FolderExplorerContext.ContextKey]
            .InvokeOnFocusRequestedEventHandler();
    }
    
    [EffectMethod(typeof(FocusSolutionExplorerAction))]
    public async Task HandleFocusSolutionExplorerAction(IDispatcher dispatcher)
    {
        _contextStateWrap.Value.ContextRecords[ContextFacts.SolutionExplorerContext.ContextKey]
            .InvokeOnFocusRequestedEventHandler();
    }
    
    [EffectMethod(typeof(FocusDialogDisplayAction))]
    public async Task HandleFocusDialogDisplayAction(IDispatcher dispatcher)
    {
        _contextStateWrap.Value.ContextRecords[ContextFacts.DialogDisplayContext.ContextKey]
            .InvokeOnFocusRequestedEventHandler();
    }
    
    [EffectMethod(typeof(FocusToolbarDisplayAction))]
    public async Task HandleFocusToolbarDisplayAction(IDispatcher dispatcher)
    {
        _contextStateWrap.Value.ContextRecords[ContextFacts.ToolbarDisplayContext.ContextKey]
            .InvokeOnFocusRequestedEventHandler();
    }
    
    [EffectMethod(typeof(FocusEditorDisplayAction))]
    public async Task HandleFocusEditorDisplayAction(IDispatcher dispatcher)
    {
        _contextStateWrap.Value.ContextRecords[ContextFacts.EditorDisplayContext.ContextKey]
            .InvokeOnFocusRequestedEventHandler();
    }
}