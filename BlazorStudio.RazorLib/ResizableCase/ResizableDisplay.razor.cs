using BlazorStudio.ClassLib.Dimensions;
using BlazorStudio.ClassLib.Resize;
using BlazorStudio.ClassLib.Store.DragCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.ResizableCase;

public partial class ResizableDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<Task> ReRenderFuncAsync { get; set; } = null!;

    public const double RESIZE_HANDLE_SQUARE_PIXELS = 10;

    private Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task>? _dragEventHandler;
    private MouseEventArgs? _previousDragMouseEventArgs;

    private ElementDimensions _northResizeHandleDimensions = new();
    private ElementDimensions _eastResizeHandleDimensions = new();
    private ElementDimensions _southResizeHandleDimensions = new();
    private ElementDimensions _westResizeHandleDimensions = new();
    private ElementDimensions _northEastResizeHandleDimensions = new();
    private ElementDimensions _southEastResizeHandleDimensions = new();
    private ElementDimensions _southWestResizeHandleDimensions = new();
    private ElementDimensions _northWestResizeHandleDimensions = new();

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;

        base.OnInitialized();
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
                await ReRenderFuncAsync.Invoke();
            }
        }
    }

    private void SubscribeToDragEvent(
        Func<(MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs), Task> dragEventHandler)
    {
        _dragEventHandler = dragEventHandler;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }
    
    public void SubscribeToDragEventWithMoveHandle()
    {
        SubscribeToDragEvent(DragEventHandlerMoveHandleAsync);
    }

    #region ResizeHandleStyleCss

    private string GetNorthResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        var parentElementHeight = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        var parentElementLeft = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

        var parentElementTop = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

        _northResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _northResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnits.Clear();

            resizeHandleWidth.DimensionUnits.AddRange(parentElementWidth.DimensionUnits);

            // width: calc(60vw - 42px);
            resizeHandleWidth.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        // Height
        {
            var resizeHandleHeight = _northResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnits.Clear();

            resizeHandleHeight.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Left
        {
            var resizeHandleLeft = _northResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnits.Clear();

            resizeHandleLeft.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Top
        {
            var resizeHandleTop = _northResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnits.Clear();

            resizeHandleTop.DimensionUnits.Add(new DimensionUnit
            {
                Value = -1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        return _northResizeHandleDimensions.StyleString;
    }

    private string GetEastResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        var parentElementHeight = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        var parentElementLeft = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

        var parentElementTop = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

        _eastResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _eastResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnits.Clear();

            resizeHandleWidth.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Height
        {
            var resizeHandleHeight = _eastResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnits.Clear();

            resizeHandleHeight.DimensionUnits.AddRange(parentElementHeight.DimensionUnits);

            resizeHandleHeight.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        // Left
        {
            var resizeHandleLeft = _eastResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnits.Clear();

            resizeHandleLeft.DimensionUnits.AddRange(parentElementWidth.DimensionUnits);

            resizeHandleLeft.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        // Top
        {
            var resizeHandleTop = _eastResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnits.Clear();

            resizeHandleTop.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        return _eastResizeHandleDimensions.StyleString;
    }

    private string GetSouthResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        var parentElementHeight = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        var parentElementLeft = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

        var parentElementTop = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

        _southResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _southResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnits.Clear();

            resizeHandleWidth.DimensionUnits.AddRange(parentElementWidth.DimensionUnits);

            resizeHandleWidth.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        // Height
        {
            var resizeHandleHeight = _southResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnits.Clear();

            resizeHandleHeight.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Left
        {
            var resizeHandleLeft = _southResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnits.Clear();

            resizeHandleLeft.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Top
        {
            var resizeHandleTop = _southResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnits.Clear();

            resizeHandleTop.DimensionUnits.AddRange(parentElementHeight.DimensionUnits);

            resizeHandleTop.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        return _southResizeHandleDimensions.StyleString;
    }

    private string GetWestResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        var parentElementHeight = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        var parentElementLeft = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

        var parentElementTop = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

        _westResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _westResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnits.Clear();

            resizeHandleWidth.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Height
        {
            var resizeHandleHeight = _westResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnits.Clear();

            resizeHandleHeight.DimensionUnits.AddRange(parentElementHeight.DimensionUnits);

            resizeHandleHeight.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        // Left
        {
            var resizeHandleLeft = _westResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnits.Clear();

            resizeHandleLeft.DimensionUnits.Add(new DimensionUnit
            {
                Value = -1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Top
        {
            var resizeHandleTop = _westResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnits.Clear();

            resizeHandleTop.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        return _westResizeHandleDimensions.StyleString;
    }

    private string GetNorthEastResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        var parentElementHeight = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        var parentElementLeft = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

        var parentElementTop = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

        _northEastResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _northEastResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnits.Clear();

            resizeHandleWidth.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Height
        {
            var resizeHandleHeight = _northEastResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnits.Clear();

            resizeHandleHeight.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Left
        {
            var resizeHandleLeft = _northEastResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnits.Clear();

            resizeHandleLeft.DimensionUnits.AddRange(parentElementWidth.DimensionUnits);

            resizeHandleLeft.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        // Top
        {
            var resizeHandleTop = _northEastResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnits.Clear();

            resizeHandleTop.DimensionUnits.Add(new DimensionUnit
            {
                Value = -1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        return _northEastResizeHandleDimensions.StyleString;
    }

    private string GetSouthEastResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        var parentElementHeight = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        var parentElementLeft = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

        var parentElementTop = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

        _southEastResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _southEastResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnits.Clear();

            resizeHandleWidth.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Height
        {
            var resizeHandleHeight = _southEastResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnits.Clear();

            resizeHandleHeight.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Left
        {
            var resizeHandleLeft = _southEastResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnits.Clear();

            resizeHandleLeft.DimensionUnits.AddRange(parentElementWidth.DimensionUnits);

            resizeHandleLeft.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        // Top
        {
            var resizeHandleTop = _southEastResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnits.Clear();

            resizeHandleTop.DimensionUnits.AddRange(parentElementHeight.DimensionUnits);

            resizeHandleTop.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        return _southEastResizeHandleDimensions.StyleString;
    }

    private string GetSouthWestResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        var parentElementHeight = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        var parentElementLeft = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

        var parentElementTop = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

        _southWestResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _southWestResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnits.Clear();

            resizeHandleWidth.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Height
        {
            var resizeHandleHeight = _southWestResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnits.Clear();

            resizeHandleHeight.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Left
        {
            var resizeHandleLeft = _southWestResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnits.Clear();

            resizeHandleLeft.DimensionUnits.Add(new DimensionUnit
            {
                Value = -1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Top
        {
            var resizeHandleTop = _southWestResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnits.Clear();

            resizeHandleTop.DimensionUnits.AddRange(parentElementHeight.DimensionUnits);

            resizeHandleTop.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            });
        }

        return _southWestResizeHandleDimensions.StyleString;
    }

    private string GetNorthWestResizeHandleStyleCss()
    {
        var parentElementWidth = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

        var parentElementHeight = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

        var parentElementLeft = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

        var parentElementTop = ElementDimensions.DimensionAttributes
            .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

        _northWestResizeHandleDimensions.ElementPositionKind = ElementPositionKind.Absolute;

        // Width
        {
            var resizeHandleWidth = _northWestResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            resizeHandleWidth.DimensionUnits.Clear();

            resizeHandleWidth.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Height
        {
            var resizeHandleHeight = _northWestResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            resizeHandleHeight.DimensionUnits.Clear();

            resizeHandleHeight.DimensionUnits.Add(new DimensionUnit
            {
                Value = RESIZE_HANDLE_SQUARE_PIXELS,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Left
        {
            var resizeHandleLeft = _northWestResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            resizeHandleLeft.DimensionUnits.Clear();

            resizeHandleLeft.DimensionUnits.Add(new DimensionUnit
            {
                Value = -1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        // Top
        {
            var resizeHandleTop = _northWestResizeHandleDimensions.DimensionAttributes
                .Single(x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            resizeHandleTop.DimensionUnits.Clear();

            resizeHandleTop.DimensionUnits.Add(new DimensionUnit
            {
                Value = -1 * RESIZE_HANDLE_SQUARE_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels
            });
        }

        return _northWestResizeHandleDimensions.StyleString;
    }

    #endregion

    #region DragEventHandlers

    private async Task DragEventHandlerNorthResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeService.ResizeNorth(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerEastResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeService.ResizeEast(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerSouthResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeService.ResizeSouth(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerWestResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeService.ResizeWest(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerNorthEastResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeService.ResizeNorthEast(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerSouthEastResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeService.ResizeSouthEast(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerSouthWestResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeService.ResizeSouthWest(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    private async Task DragEventHandlerNorthWestResizeHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeService.ResizeNorthWest(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }
    
    private async Task DragEventHandlerMoveHandleAsync(
        (MouseEventArgs firstMouseEventArgs, MouseEventArgs secondMouseEventArgs) mouseEventArgsTuple)
    {
        ResizeService.Move(
            ElementDimensions,
            mouseEventArgsTuple.firstMouseEventArgs,
            mouseEventArgsTuple.secondMouseEventArgs);

        await InvokeAsync(StateHasChanged);
    }

    #endregion

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
    }
}