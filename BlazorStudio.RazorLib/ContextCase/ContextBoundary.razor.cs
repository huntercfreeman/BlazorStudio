using System.Collections.Immutable;
using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.Store.ContextCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.ContextCase;

public partial class ContextBoundary : ComponentBase, IDisposable
{
    [Inject]
    private IStateSelection<ContextState, ContextRecord> ContextStateSelector { get; set; } = null!;    
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [CascadingParameter]
    public ContextBoundary? ParentContextBoundary { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ContextKey ContextKey { get; set; } = null!;
    [Parameter, EditorRequired]
    public string CssClassString { get; set; } = null!;
    [Parameter, EditorRequired]
    public string CssStyleString { get; set; } = null!;
    [Parameter, EditorRequired]
    public int TabIndex { get; set; } = -1;
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;
    [Parameter]
    public Func<FocusEventArgs?, Task>? OnFocusIn { get; set; }
    [Parameter]
    public Func<KeyboardEventArgs, Task>? OnKeyUp { get; set; }
    [Parameter]
    public bool OnKeyDownPreventDefault { get; set; }

    private ElementReference? _contextBoundaryElementReference;
    
    protected override void OnInitialized()
    {
        ContextStateSelector.Select(x => x.ContextRecords[ContextKey]);
        
        base.OnInitialized();
    }
    
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            ContextStateSelector.Value.OnFocusRequestedEventHandler += ValueOnOnFocusRequestedEventHandler;
        }
        
        base.OnAfterRender(firstRender);
    }

    private async void ValueOnOnFocusRequestedEventHandler(object? sender, EventArgs e)
    {
        if (_contextBoundaryElementReference is not null)
            await _contextBoundaryElementReference.Value.FocusAsync();
    }

    public void DispatchSetActiveContextStatesAction(List<ContextRecord> contextRecords)
    {
        contextRecords.Add(ContextStateSelector.Value); 
        
        if (ParentContextBoundary is not null)
        {
            ParentContextBoundary.DispatchSetActiveContextStatesAction(contextRecords);
        }
        else
        {
            Dispatcher.Dispatch(new SetActiveContextStatesAction(contextRecords.ToImmutableList()));
        }
    }
    
    public async Task HandleOnFocusInAsync(FocusEventArgs? focusEventArgs)
    {
        DispatchSetActiveContextStatesAction(new());

        if (OnFocusIn is not null)
            await OnFocusIn.Invoke(focusEventArgs);
    }
    
    public async Task HandleOnKeyUpAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (OnKeyUp is not null)
            await OnKeyUp.Invoke(keyboardEventArgs);
    }

    public void Dispose()
    {
        ContextStateSelector.Value.OnFocusRequestedEventHandler -= ValueOnOnFocusRequestedEventHandler;
    }
}

