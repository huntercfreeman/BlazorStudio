using BlazorALaCarte.Shared.Drag;
using BlazorStudio.ClassLib.Panel;
using BlazorStudio.ClassLib.Store.PanelCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Panel;

public partial class PanelTabDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IState<PanelsCollection> PanelsCollectionWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public PanelTab PanelTab { get; set; } = null!;
    [Parameter, EditorRequired]
    public PanelRecord PanelRecord { get; set; } = null!;

    private string IsActiveCssClassString => PanelRecord.ActivePanelTabKey == PanelTab.PanelTabKey
        ? "balc_active"
        : string.Empty;

    private readonly SemaphoreSlim _onMouseMoveSemaphoreSlim = new(1, 1);
    private readonly TimeSpan _onMouseMoveDelay = TimeSpan.FromMilliseconds(25);
    
    private bool _thinksLeftMouseButtonIsDown;
    
    private Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task>? _dragEventHandler;
    private MouseEventArgs? _previousDragMouseEventArgs;

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;
        PanelsCollectionWrap.StateChanged += PanelsCollectionWrapOnStateChanged;
        
        base.OnInitialized();
    }

    private void PanelsCollectionWrapOnStateChanged(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    private void DispatchSetActivePanelTabActionOnClick()
    {
        Dispatcher.Dispatch(new PanelsCollection.SetActivePanelTabAction(
            PanelRecord.PanelRecordKey,
            PanelTab.PanelTabKey));
    }
    
    private Task HandleOnMouseDownAsync(MouseEventArgs arg)
    {
        _thinksLeftMouseButtonIsDown = true;
        SubscribeToDragEventForScrolling();

        return Task.CompletedTask;
    }
    
    private async void DragStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (!DragStateWrap.Value.ShouldDisplay)
        {
            _dragEventHandler = null;
            _previousDragMouseEventArgs = null;
        }
        else
        {
            var mouseEventArgs = DragStateWrap.Value.MouseEventArgs;

            if (_dragEventHandler is not null)
            {
                if (_previousDragMouseEventArgs is not null &&
                    mouseEventArgs is not null)
                {
                    await _dragEventHandler.Invoke((_previousDragMouseEventArgs, mouseEventArgs));
                }

                _previousDragMouseEventArgs = mouseEventArgs;
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    public void SubscribeToDragEventForScrolling()
    {
        _dragEventHandler = DragEventHandlerScrollAsync;
        
        Dispatcher.Dispatch(new PanelsCollection.SetThinksIsBeingDraggedAction(true));
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }
    
    private async Task DragEventHandlerScrollAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        var success = await _onMouseMoveSemaphoreSlim
            .WaitAsync(TimeSpan.Zero);
    
        if (!success)
            return;
    
        try
        {
            // Buttons is a bit flag
            // '& 1' gets if left mouse button is held
            if (_thinksLeftMouseButtonIsDown &&
                (mouseEventArgsTuple.secondMouseEventArgs.Buttons & 1) == 1)
            {
            }
            else
            {
                _thinksLeftMouseButtonIsDown = false;
            }
    
            await Task.Delay(_onMouseMoveDelay);
        }
        finally
        {
            _onMouseMoveSemaphoreSlim.Release();
        }
    }

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
        PanelsCollectionWrap.StateChanged -= PanelsCollectionWrapOnStateChanged;
    }
}