using System.Collections.Immutable;
using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.QuickSelectCase;
using Fluxor;

namespace BlazorStudio.ClassLib.Store.CommandCase.Focus;

public class FocusEffects
{
    private readonly IState<ContextState> _contextStateWrap;
    private readonly IState<DialogStates> _dialogStatesWrap;

    public FocusEffects(IState<ContextState> contextStateWrap,
        IState<DialogStates> dialogStatesWrap)
    {
        _contextStateWrap = contextStateWrap;
        _dialogStatesWrap = dialogStatesWrap;
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
    
    [EffectMethod(typeof(FocusTerminalDisplayAction))]
    public async Task HandleFocusTerminalDisplayAction(IDispatcher dispatcher)
    {
        _contextStateWrap.Value.ContextRecords[ContextFacts.TerminalDisplayContext.ContextKey]
            .InvokeOnFocusRequestedEventHandler();
    }
            
    [EffectMethod(typeof(FocusDialogQuickSelectDisplayAction))]
    public async Task HandleFocusDialogQuickSelectDisplayAction(IDispatcher dispatcher)
    {
        Console.WriteLine("FocusEffects HandleFocusDialogQuickSelectDisplayAction");
        
        var quickSelectItems = _dialogStatesWrap.Value.List
            .Select(x => (IQuickSelectItem) new QuickSelectItem<DialogRecord>(x.Title, x))
            .ToImmutableArray();

        var quickSelectState = new QuickSelectState
        {
            IsDisplayed = true,
            QuickSelectItems = quickSelectItems,
            OnItemSelectedFunc = (dialogRecord) =>
            {
                ((DialogRecord)dialogRecord.ItemNoType).InvokeOnFocusRequestedEventHandler();
                return Task.CompletedTask;
            },
            OnHoveredItemChangedFunc = (item) => Task.CompletedTask
        };
        
        dispatcher.Dispatch(new SetQuickSelectStateAction(quickSelectState));
    }
}