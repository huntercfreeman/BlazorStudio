using System.Collections.Immutable;
using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.RazorLib.ShouldRender;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.ContextCase;

public partial class ContextBoundary : ComponentBase, IDisposable
{
    private ElementReference? _contextBoundaryElementReference;
    [Inject]
    private IStateSelection<ContextState, ContextRecord> ContextStateSelector { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public ContextBoundary? ParentContextBoundary { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public ContextKey ContextKey { get; set; } = null!;
    [Parameter]
    [EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;
    [Parameter]
    public string CssClassString { get; set; } = null!;
    [Parameter]
    public string CssStyleString { get; set; } = null!;
    [Parameter]
    public int TabIndex { get; set; } = -1;
    /// <summary>
    ///     <see cref="IsIsland" /> set to true means the Blazor component
    ///     will only rerender due to FluxorComponent state [Inject]
    ///     (or other EventHandler like logic). And will ShouldRender => false
    ///     on any cascading StateHasChanged events from a parent component.
    /// </summary>
    [Parameter]
    public bool IsIsland { get; set; }
    [Parameter]
    public Func<FocusEventArgs?, Task>? OnFocusIn { get; set; }
    [Parameter]
    public bool IgnoreFocusRequests { get; set; }
    [Parameter]
    public Func<KeyboardEventArgs, Task>? OnKeyUp { get; set; }
    [Parameter]
    public bool OnKeyDownPreventDefault { get; set; }

    public ContextRecord GetContextState => ContextStateSelector.Value;

    public void Dispose()
    {
        ContextStateSelector.Value.OnFocusRequestedEventHandler -= ValueOnOnFocusRequestedEventHandler;
    }

    protected override void OnInitialized()
    {
        ContextStateSelector.Select(x => x.ContextRecords[ContextKey]);

        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            if (!IgnoreFocusRequests)
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
            ParentContextBoundary.DispatchSetActiveContextStatesAction(contextRecords);
        else
            Dispatcher.Dispatch(new SetActiveContextStatesAction(contextRecords.ToImmutableList()));
    }

    public async Task HandleOnFocusInAsync(FocusEventArgs? focusEventArgs)
    {
        DispatchSetActiveContextStatesAction(new List<ContextRecord>());

        if (OnFocusIn is not null)
            await OnFocusIn.Invoke(focusEventArgs);
    }

    public async Task HandleOnKeyUpAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (OnKeyUp is not null)
            await OnKeyUp.Invoke(keyboardEventArgs);
    }

    public bool HandleShouldRenderBoundary(ShouldRenderBoundary.IsFirstShouldRenderValue isFirstShouldRenderValue)
    {
        if (!IsIsland)
            return true;

        if (isFirstShouldRenderValue.IsFirstShouldRender)
            return true;

        return false;
    }
}