using BlazorStudio.ClassLib.Store.DragCase;
using BlazorStudio.ClassLib.Store.TransformableCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Transformable;

public partial class TransformableDisplay : ComponentBase, IDisposable
{
    private readonly SemaphoreSlim _transformableDisplaySemaphoreSlim = new(1, 1);

    private Func<MouseEventArgs, Task>? _dragStateEventHandler;
    private Dimensions _eastResizeHandleDimensions = new();

    // Diagonal Resize Handles
    private Dimensions _northEastResizeHandleDimensions = new();

    // Cardinal Resize Handles
    private Dimensions _northResizeHandleDimensions = new();
    private Dimensions _northWestResizeHandleDimensions = new();
    private MouseEventArgs? _previousDragMouseEventArgs;

    private int _resizeEventCounter;
    private Dimensions _southEastResizeHandleDimensions = new();
    private Dimensions _southResizeHandleDimensions = new();
    private Dimensions _southWestResizeHandleDimensions = new();
    private Dimensions _westResizeHandleDimensions = new();
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IState<TransformableOptionsState> TransformableOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;
    [Parameter]
    [EditorRequired]
    public Func<Task> ReRenderFunc { get; set; } = null!;

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragState_StateChanged;
        TransformableOptionsStateWrap.StateChanged -= TransformableOptionsStateWrapOnStateChanged;
    }

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragState_StateChanged;
        TransformableOptionsStateWrap.StateChanged += TransformableOptionsStateWrapOnStateChanged;

        base.OnInitialized();
    }

    private async void DragState_StateChanged(object? sender, EventArgs e)
    {
        if (!DragStateWrap.Value.IsDisplayed)
        {
            _dragStateEventHandler = null;
            _previousDragMouseEventArgs = null;
        }
        else
        {
            var success = await _transformableDisplaySemaphoreSlim
                .WaitAsync(TimeSpan.Zero);

            if (!success)
                return;

            try
            {
                var mouseEventArgs = DragStateWrap.Value.MouseEventArgs;

                if (_dragStateEventHandler is not null)
                {
                    if (_previousDragMouseEventArgs is not null &&
                        mouseEventArgs is not null)
                    {
                        await _dragStateEventHandler(mouseEventArgs);
                    }

                    _previousDragMouseEventArgs = mouseEventArgs;
                    await ReRenderFunc();
                    await InvokeAsync(StateHasChanged);
                }
            }
            finally
            {
                _transformableDisplaySemaphoreSlim.Release();
            }
        }
    }

    private void TransformableOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    private void SubscribeToDragEventWithNorthResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerNorthResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private Task DragEventHandlerNorthResizeHandle(MouseEventArgs mouseEventArgs)
    {
        _resizeEventCounter++;

        // HEIGHT
        var heightPixelOffset =
            Dimensions.HeightCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (heightPixelOffset is null)
        {
            heightPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.HeightCalc.Add(heightPixelOffset);
        }

        var deltaY = mouseEventArgs.ClientY - _previousDragMouseEventArgs!.ClientY;

        heightPixelOffset.Value -= deltaY;

        // TOP
        var topPixelOffset = Dimensions.TopCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (topPixelOffset is null)
        {
            topPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.TopCalc.Add(topPixelOffset);
        }

        topPixelOffset.Value += deltaY;
        return Task.CompletedTask;
    }

    private void SubscribeToDragEventWithEastResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerEastResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private Task DragEventHandlerEastResizeHandle(MouseEventArgs mouseEventArgs)
    {
        var widthPixelOffset =
            Dimensions.WidthCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (widthPixelOffset is null)
        {
            widthPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.WidthCalc.Add(widthPixelOffset);
        }

        var deltaX = mouseEventArgs.ClientX - _previousDragMouseEventArgs!.ClientX;

        widthPixelOffset.Value += deltaX;
        return Task.CompletedTask;
    }

    private void SubscribeToDragEventWithSouthResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerSouthResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private Task DragEventHandlerSouthResizeHandle(MouseEventArgs mouseEventArgs)
    {
        var heightPixelOffset =
            Dimensions.HeightCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (heightPixelOffset is null)
        {
            heightPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.HeightCalc.Add(heightPixelOffset);
        }

        var deltaY = mouseEventArgs.ClientY - _previousDragMouseEventArgs!.ClientY;

        heightPixelOffset.Value += deltaY;
        return Task.CompletedTask;
    }

    private void SubscribeToDragEventWithWestResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerWestResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private Task DragEventHandlerWestResizeHandle(MouseEventArgs mouseEventArgs)
    {
        var widthPixelOffset =
            Dimensions.WidthCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (widthPixelOffset is null)
        {
            widthPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.WidthCalc.Add(widthPixelOffset);
        }

        var deltaX = mouseEventArgs.ClientX - _previousDragMouseEventArgs!.ClientX;

        widthPixelOffset.Value -= deltaX;

        var leftPixelOffset = Dimensions.LeftCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (leftPixelOffset is null)
        {
            leftPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.LeftCalc.Add(leftPixelOffset);
        }

        leftPixelOffset.Value += deltaX;
        return Task.CompletedTask;
    }

    private void SubscribeToDragEventWithNorthEastResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerNorthEastResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private Task DragEventHandlerNorthEastResizeHandle(MouseEventArgs mouseEventArgs)
    {
        var widthPixelOffset =
            Dimensions.WidthCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (widthPixelOffset is null)
        {
            widthPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.WidthCalc.Add(widthPixelOffset);
        }

        var deltaX = mouseEventArgs.ClientX - _previousDragMouseEventArgs!.ClientX;

        widthPixelOffset.Value += deltaX;

        var heightPixelOffset =
            Dimensions.HeightCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (heightPixelOffset is null)
        {
            heightPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.HeightCalc.Add(heightPixelOffset);
        }

        var deltaY = mouseEventArgs.ClientY - _previousDragMouseEventArgs!.ClientY;

        heightPixelOffset.Value -= deltaY;

        var topPixelOffset = Dimensions.TopCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (topPixelOffset is null)
        {
            topPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.TopCalc.Add(topPixelOffset);
        }

        topPixelOffset.Value += deltaY;
        return Task.CompletedTask;
    }

    private void SubscribeToDragEventWithSouthEastResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerSouthEastResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private Task DragEventHandlerSouthEastResizeHandle(MouseEventArgs mouseEventArgs)
    {
        var heightPixelOffset =
            Dimensions.HeightCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (heightPixelOffset is null)
        {
            heightPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.HeightCalc.Add(heightPixelOffset);
        }

        var deltaY = mouseEventArgs.ClientY - _previousDragMouseEventArgs!.ClientY;

        heightPixelOffset.Value += deltaY;

        var widthPixelOffset =
            Dimensions.WidthCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (widthPixelOffset is null)
        {
            widthPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.WidthCalc.Add(widthPixelOffset);
        }

        var deltaX = mouseEventArgs.ClientX - _previousDragMouseEventArgs!.ClientX;

        widthPixelOffset.Value += deltaX;
        return Task.CompletedTask;
    }

    private void SubscribeToDragEventWithSouthWestResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerSouthWestResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private Task DragEventHandlerSouthWestResizeHandle(MouseEventArgs mouseEventArgs)
    {
        var heightPixelOffset =
            Dimensions.HeightCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (heightPixelOffset is null)
        {
            heightPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.HeightCalc.Add(heightPixelOffset);
        }

        var deltaY = mouseEventArgs.ClientY - _previousDragMouseEventArgs!.ClientY;

        heightPixelOffset.Value += deltaY;

        var widthPixelOffset =
            Dimensions.WidthCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (widthPixelOffset is null)
        {
            widthPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.WidthCalc.Add(widthPixelOffset);
        }

        var deltaX = mouseEventArgs.ClientX - _previousDragMouseEventArgs!.ClientX;

        widthPixelOffset.Value -= deltaX;

        var leftPixelOffset = Dimensions.LeftCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (leftPixelOffset is null)
        {
            leftPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.LeftCalc.Add(leftPixelOffset);
        }

        leftPixelOffset.Value += deltaX;
        return Task.CompletedTask;
    }

    private void SubscribeToDragEventWithNorthWestResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerNorthWestResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private Task DragEventHandlerNorthWestResizeHandle(MouseEventArgs mouseEventArgs)
    {
        var heightPixelOffset =
            Dimensions.HeightCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (heightPixelOffset is null)
        {
            heightPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.HeightCalc.Add(heightPixelOffset);
        }

        var deltaY = mouseEventArgs.ClientY - _previousDragMouseEventArgs!.ClientY;

        heightPixelOffset.Value -= deltaY;

        var topPixelOffset = Dimensions.TopCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (topPixelOffset is null)
        {
            topPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.TopCalc.Add(topPixelOffset);
        }

        topPixelOffset.Value += deltaY;

        var widthPixelOffset =
            Dimensions.WidthCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (widthPixelOffset is null)
        {
            widthPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.WidthCalc.Add(widthPixelOffset);
        }

        var deltaX = mouseEventArgs.ClientX - _previousDragMouseEventArgs!.ClientX;

        widthPixelOffset.Value -= deltaX;

        var leftPixelOffset = Dimensions.LeftCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (leftPixelOffset is null)
        {
            leftPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.LeftCalc.Add(leftPixelOffset);
        }

        leftPixelOffset.Value += deltaX;
        return Task.CompletedTask;
    }

    public void SubscribeToDragEventWithMoveHandle()
    {
        _dragStateEventHandler = DragEventHandlerMoveHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private Task DragEventHandlerMoveHandle(MouseEventArgs mouseEventArgs)
    {
        var deltaY = mouseEventArgs.ClientY - _previousDragMouseEventArgs!.ClientY;

        var topPixelOffset = Dimensions.TopCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (topPixelOffset is null)
        {
            topPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.TopCalc.Add(topPixelOffset);
        }

        topPixelOffset.Value += deltaY;

        var deltaX = mouseEventArgs.ClientX - _previousDragMouseEventArgs!.ClientX;

        var leftPixelOffset = Dimensions.LeftCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (leftPixelOffset is null)
        {
            leftPixelOffset = new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0,
            };

            Dimensions.LeftCalc.Add(leftPixelOffset);
        }

        leftPixelOffset.Value += deltaX;
        return Task.CompletedTask;
    }

    #region HandleCssStylings

    private string GetNorthResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new(Dimensions.WidthCalc)
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> heightDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> leftDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        List<DimensionUnit> topDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        _northResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _northResizeHandleDimensions.WidthCalc.Clear();
        _northResizeHandleDimensions.WidthCalc.AddRange(widthDimension);
        
        _northResizeHandleDimensions.HeightCalc.Clear();
        _northResizeHandleDimensions.HeightCalc.AddRange(heightDimension);
        
        _northResizeHandleDimensions.LeftCalc.Clear();
        _northResizeHandleDimensions.LeftCalc.AddRange(leftDimension);
        
        _northResizeHandleDimensions.TopCalc.Clear();
        _northResizeHandleDimensions.TopCalc.AddRange(topDimension);

        return _northResizeHandleDimensions.DimensionsCssString;
    }

    private string GetEastResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> heightDimension = new(Dimensions.HeightCalc)
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> leftDimension = new(Dimensions.WidthCalc)
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        List<DimensionUnit> topDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        _eastResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _eastResizeHandleDimensions.WidthCalc.Clear();
        _eastResizeHandleDimensions.WidthCalc.AddRange(widthDimension);
        
        _eastResizeHandleDimensions.HeightCalc.Clear();
        _eastResizeHandleDimensions.HeightCalc.AddRange(heightDimension);
        
        _eastResizeHandleDimensions.LeftCalc.Clear();
        _eastResizeHandleDimensions.LeftCalc.AddRange(leftDimension);
        
        _eastResizeHandleDimensions.TopCalc.Clear();
        _eastResizeHandleDimensions.TopCalc.AddRange(topDimension);
        

        return _eastResizeHandleDimensions.DimensionsCssString;
    }

    private string GetSouthResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new(Dimensions.WidthCalc)
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> heightDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> leftDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        List<DimensionUnit> topDimension = new(Dimensions.HeightCalc)
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        _southResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _southResizeHandleDimensions.WidthCalc.Clear();
        _southResizeHandleDimensions.WidthCalc.AddRange(widthDimension);
        
        _southResizeHandleDimensions.HeightCalc.Clear();
        _southResizeHandleDimensions.HeightCalc.AddRange(heightDimension);
        
        _southResizeHandleDimensions.LeftCalc.Clear();
        _southResizeHandleDimensions.LeftCalc.AddRange(leftDimension);
        
        _southResizeHandleDimensions.TopCalc.Clear();
        _southResizeHandleDimensions.TopCalc.AddRange(topDimension);
        

        return _southResizeHandleDimensions.DimensionsCssString;
    }

    private string GetWestResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> heightDimension = new(Dimensions.HeightCalc)
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> leftDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        List<DimensionUnit> topDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        _westResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _westResizeHandleDimensions.WidthCalc.Clear();
        _westResizeHandleDimensions.WidthCalc.AddRange(widthDimension);
        
        _westResizeHandleDimensions.HeightCalc.Clear();
        _westResizeHandleDimensions.HeightCalc.AddRange(heightDimension);
        
        _westResizeHandleDimensions.LeftCalc.Clear();
        _westResizeHandleDimensions.LeftCalc.AddRange(leftDimension);
        
        _westResizeHandleDimensions.TopCalc.Clear();
        _westResizeHandleDimensions.TopCalc.AddRange(topDimension);
        

        return _westResizeHandleDimensions.DimensionsCssString;
    }

    private string GetNorthEastResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> heightDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> leftDimension = new(Dimensions.WidthCalc)
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        List<DimensionUnit> topDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        _northEastResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _northEastResizeHandleDimensions.WidthCalc.Clear();
        _northEastResizeHandleDimensions.WidthCalc.AddRange(widthDimension);
        
        _northEastResizeHandleDimensions.HeightCalc.Clear();
        _northEastResizeHandleDimensions.HeightCalc.AddRange(heightDimension);
        
        _northEastResizeHandleDimensions.LeftCalc.Clear();
        _northEastResizeHandleDimensions.LeftCalc.AddRange(leftDimension);
        
        _northEastResizeHandleDimensions.TopCalc.Clear();
        _northEastResizeHandleDimensions.TopCalc.AddRange(topDimension);
        

        return _northEastResizeHandleDimensions.DimensionsCssString;
    }

    private string GetSouthEastResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> heightDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> leftDimension = new(Dimensions.WidthCalc)
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        List<DimensionUnit> topDimension = new(Dimensions.HeightCalc)
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        _southEastResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _southEastResizeHandleDimensions.WidthCalc.Clear();
        _southEastResizeHandleDimensions.WidthCalc.AddRange(widthDimension);
        
        _southEastResizeHandleDimensions.HeightCalc.Clear();
        _southEastResizeHandleDimensions.HeightCalc.AddRange(heightDimension);
        
        _southEastResizeHandleDimensions.LeftCalc.Clear();
        _southEastResizeHandleDimensions.LeftCalc.AddRange(leftDimension);
        
        _southEastResizeHandleDimensions.TopCalc.Clear();
        _southEastResizeHandleDimensions.TopCalc.AddRange(topDimension);
        

        return _southEastResizeHandleDimensions.DimensionsCssString;
    }

    private string GetSouthWestResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> heightDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> leftDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        List<DimensionUnit> topDimension = new(Dimensions.HeightCalc)
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        _southWestResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _southWestResizeHandleDimensions.WidthCalc.Clear();
        _southWestResizeHandleDimensions.WidthCalc.AddRange(widthDimension);
        
        _southWestResizeHandleDimensions.HeightCalc.Clear();
        _southWestResizeHandleDimensions.HeightCalc.AddRange(heightDimension);
        
        _southWestResizeHandleDimensions.LeftCalc.Clear();
        _southWestResizeHandleDimensions.LeftCalc.AddRange(leftDimension);
        
        _southWestResizeHandleDimensions.TopCalc.Clear();
        _southWestResizeHandleDimensions.TopCalc.AddRange(topDimension);
        

        return _southWestResizeHandleDimensions.DimensionsCssString;
    }

    private string GetNorthWestResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> heightDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value,
            },
        };

        List<DimensionUnit> leftDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        List<DimensionUnit> topDimension = new()
        {
            new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0,
            },
        };

        _northWestResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _northWestResizeHandleDimensions.WidthCalc.Clear();
        _northWestResizeHandleDimensions.WidthCalc.AddRange(widthDimension);
        
        _northWestResizeHandleDimensions.HeightCalc.Clear();
        _northWestResizeHandleDimensions.HeightCalc.AddRange(heightDimension);
        
        _northWestResizeHandleDimensions.LeftCalc.Clear();
        _northWestResizeHandleDimensions.LeftCalc.AddRange(leftDimension);
        
        _northWestResizeHandleDimensions.TopCalc.Clear();
        _northWestResizeHandleDimensions.TopCalc.AddRange(topDimension);
        

        return _northWestResizeHandleDimensions.DimensionsCssString;
    }

    #endregion
}