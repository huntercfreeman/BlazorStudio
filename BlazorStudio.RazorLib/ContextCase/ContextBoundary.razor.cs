using System.Collections.Immutable;
using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.Store.ContextCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.ContextCase;

public partial class ContextBoundary : ComponentBase
{
    [Inject]
    private IState<ContextState> ContextStateWrap { get; set; } = null!;    
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [CascadingParameter]
    public ContextBoundary? ParentContextBoundary { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ContextRecord ContextRecord { get; set; } = null!;
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;

    public void DispatchSetActiveContextStatesAction(List<ContextRecord> contextRecords)
    {
        contextRecords.Add(ContextRecord); 
        
        if (ParentContextBoundary is not null)
        {
            ParentContextBoundary.DispatchSetActiveContextStatesAction(contextRecords);
        }
        else
        {
            Dispatcher.Dispatch(new SetActiveContextStatesAction(contextRecords.ToImmutableList()));
        }
    }
}

