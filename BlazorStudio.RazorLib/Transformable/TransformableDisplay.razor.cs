using System.Text;
using BlazorStudio.ClassLib.Store.DragCase;
using BlazorStudio.ClassLib.Store.TransformableCase;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Transformable;

public partial class TransformableDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject] 
    private IState<TransformableOptionsState> TransformableOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<Task> ReRenderFunc { get; set; } = null!;

    private Func<MouseEventArgs, Task>? _dragStateEventHandler;
    private MouseEventArgs? _previousDragMouseEventArgs;

    private int _resizeEventCounter;
    private readonly SemaphoreSlim _transformableDisplaySemaphoreSlim = new(1, 1);

    // Cardinal Resize Handles
    private Dimensions _northResizeHandleDimensions = new();
    private Dimensions _eastResizeHandleDimensions = new();
    private Dimensions _southResizeHandleDimensions = new();
    private Dimensions _westResizeHandleDimensions = new();

    // Diagonal Resize Handles
    private Dimensions _northEastResizeHandleDimensions = new();
    private Dimensions _southEastResizeHandleDimensions = new();
    private Dimensions _southWestResizeHandleDimensions = new();
    private Dimensions _northWestResizeHandleDimensions = new();

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
                    if (_previousDragMouseEventArgs is not null)
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
    
    #region HandleCssStylings
    private string GetNorthResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new(Dimensions.WidthCalc)
        {
            new() 
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };
        
        List<DimensionUnit> heightDimension = new()
        {
            new() 
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };

        List<DimensionUnit> leftDimension = new()
        {
            new() 
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };
        
        List<DimensionUnit> topDimension = new()
        {
            new() 
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };

        _northResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _northResizeHandleDimensions.WidthCalc = widthDimension;
        _northResizeHandleDimensions.HeightCalc = heightDimension;
        _northResizeHandleDimensions.LeftCalc = leftDimension;
        _northResizeHandleDimensions.TopCalc = topDimension;

        return _northResizeHandleDimensions.DimensionsCssString;
    }

    private string GetEastResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };
        
        List<DimensionUnit> heightDimension = new(Dimensions.HeightCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };
        
        List<DimensionUnit> leftDimension = new(Dimensions.WidthCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };
        
        List<DimensionUnit> topDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };

        _eastResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _eastResizeHandleDimensions.WidthCalc = widthDimension;
        _eastResizeHandleDimensions.HeightCalc = heightDimension;
        _eastResizeHandleDimensions.LeftCalc = leftDimension;
        _eastResizeHandleDimensions.TopCalc = topDimension;

        return _eastResizeHandleDimensions.DimensionsCssString;
    }

    private string GetSouthResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new(Dimensions.WidthCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };

        List<DimensionUnit> heightDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };

        List<DimensionUnit> leftDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };

        List<DimensionUnit> topDimension = new(Dimensions.HeightCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };

        _southResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _southResizeHandleDimensions.WidthCalc = widthDimension;
        _southResizeHandleDimensions.HeightCalc = heightDimension;
        _southResizeHandleDimensions.LeftCalc = leftDimension;
        _southResizeHandleDimensions.TopCalc = topDimension;

        return _southResizeHandleDimensions.DimensionsCssString;
    }

    private string GetWestResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };

        List<DimensionUnit> heightDimension = new(Dimensions.HeightCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };

        List<DimensionUnit> leftDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };

        List<DimensionUnit> topDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };

        _westResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _westResizeHandleDimensions.WidthCalc = widthDimension;
        _westResizeHandleDimensions.HeightCalc = heightDimension;
        _westResizeHandleDimensions.LeftCalc = leftDimension;
        _westResizeHandleDimensions.TopCalc = topDimension;

        return _westResizeHandleDimensions.DimensionsCssString;
    }

    private string GetNorthEastResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };
        
        List<DimensionUnit> heightDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };
        
        List<DimensionUnit> leftDimension = new(Dimensions.WidthCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };
        
        List<DimensionUnit> topDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };

        _northEastResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _northEastResizeHandleDimensions.WidthCalc = widthDimension;
        _northEastResizeHandleDimensions.HeightCalc = heightDimension;
        _northEastResizeHandleDimensions.LeftCalc = leftDimension;
        _northEastResizeHandleDimensions.TopCalc = topDimension;

        return _northEastResizeHandleDimensions.DimensionsCssString;
    }

    private string GetSouthEastResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };
        
        List<DimensionUnit> heightDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };
        
        List<DimensionUnit> leftDimension = new(Dimensions.WidthCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };
        
        List<DimensionUnit> topDimension = new(Dimensions.HeightCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };

        _southEastResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _southEastResizeHandleDimensions.WidthCalc = widthDimension;
        _southEastResizeHandleDimensions.HeightCalc = heightDimension;
        _southEastResizeHandleDimensions.LeftCalc = leftDimension;
        _southEastResizeHandleDimensions.TopCalc = topDimension;

        return _southEastResizeHandleDimensions.DimensionsCssString;
    }

    private string GetSouthWestResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };
        
        List<DimensionUnit> heightDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };
        
        List<DimensionUnit> leftDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };
        
        List<DimensionUnit> topDimension = new(Dimensions.HeightCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };

        _southWestResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _southWestResizeHandleDimensions.WidthCalc = widthDimension;
        _southWestResizeHandleDimensions.HeightCalc = heightDimension;
        _southWestResizeHandleDimensions.LeftCalc = leftDimension;
        _southWestResizeHandleDimensions.TopCalc = topDimension;

        return _southWestResizeHandleDimensions.DimensionsCssString;
    }

    private string GetNorthWestResizeHandleCssStyling()
    {
        List<DimensionUnit> widthDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };
        
        List<DimensionUnit> heightDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value
            }
        };
        
        List<DimensionUnit> leftDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };
        
        List<DimensionUnit> topDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * TransformableOptionsStateWrap.Value.ResizeHandleDimensionUnit.Value / 2.0
            }
        };

        _northWestResizeHandleDimensions.DimensionsPositionKind = DimensionsPositionKind.Absolute;

        _northWestResizeHandleDimensions.WidthCalc = widthDimension;
        _northWestResizeHandleDimensions.HeightCalc = heightDimension;
        _northWestResizeHandleDimensions.LeftCalc = leftDimension;
        _northWestResizeHandleDimensions.TopCalc = topDimension;

        return _northWestResizeHandleDimensions.DimensionsCssString;
    }
    #endregion

    private void SubscribeToDragEventWithNorthResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerNorthResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private async Task DragEventHandlerNorthResizeHandle(MouseEventArgs mouseEventArgs)
    {
        _resizeEventCounter++;

        // HEIGHT
        var heightPixelOffset = Dimensions.HeightCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (heightPixelOffset is null)
        {
            heightPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };
            
            Dimensions.HeightCalc.Add(heightPixelOffset);
        }

        var deltaY = mouseEventArgs.ClientY - _previousDragMouseEventArgs!.ClientY;

        heightPixelOffset.Value -= deltaY;

        // TOP
        var topPixelOffset = Dimensions.TopCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (topPixelOffset is null)
        {
            topPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.TopCalc.Add(topPixelOffset);
        }

        topPixelOffset.Value += deltaY;
    }

    private void SubscribeToDragEventWithEastResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerEastResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private async Task DragEventHandlerEastResizeHandle(MouseEventArgs mouseEventArgs)
    {
        var widthPixelOffset = Dimensions.WidthCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (widthPixelOffset is null)
        {
            widthPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.WidthCalc.Add(widthPixelOffset);
        }

        var deltaX = mouseEventArgs.ClientX - _previousDragMouseEventArgs!.ClientX;

        widthPixelOffset.Value += deltaX;
    }

    private void SubscribeToDragEventWithSouthResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerSouthResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private async Task DragEventHandlerSouthResizeHandle(MouseEventArgs mouseEventArgs)
    {
        var heightPixelOffset = Dimensions.HeightCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (heightPixelOffset is null)
        {
            heightPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.HeightCalc.Add(heightPixelOffset);
        }

        var deltaY = mouseEventArgs.ClientY - _previousDragMouseEventArgs!.ClientY;

        heightPixelOffset.Value += deltaY;
    }

    private void SubscribeToDragEventWithWestResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerWestResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private async Task DragEventHandlerWestResizeHandle(MouseEventArgs mouseEventArgs)
    {
        var widthPixelOffset = Dimensions.WidthCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (widthPixelOffset is null)
        {
            widthPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.WidthCalc.Add(widthPixelOffset);
        }

        var deltaX = mouseEventArgs.ClientX - _previousDragMouseEventArgs!.ClientX;

        widthPixelOffset.Value -= deltaX;
        
        var leftPixelOffset = Dimensions.LeftCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (leftPixelOffset is null)
        {
            leftPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.LeftCalc.Add(leftPixelOffset);
        }

        leftPixelOffset.Value += deltaX;
    }

    private void SubscribeToDragEventWithNorthEastResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerNorthEastResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private async Task DragEventHandlerNorthEastResizeHandle(MouseEventArgs mouseEventArgs)
    {
        var widthPixelOffset = Dimensions.WidthCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (widthPixelOffset is null)
        {
            widthPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.WidthCalc.Add(widthPixelOffset);
        }

        var deltaX = mouseEventArgs.ClientX - _previousDragMouseEventArgs!.ClientX;

        widthPixelOffset.Value += deltaX;

        var heightPixelOffset = Dimensions.HeightCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (heightPixelOffset is null)
        {
            heightPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.HeightCalc.Add(heightPixelOffset);
        }

        var deltaY = mouseEventArgs.ClientY - _previousDragMouseEventArgs!.ClientY;

        heightPixelOffset.Value -= deltaY;
        
        var topPixelOffset = Dimensions.TopCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (topPixelOffset is null)
        {
            topPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.TopCalc.Add(topPixelOffset);
        }

        topPixelOffset.Value += deltaY;
    }

    private void SubscribeToDragEventWithSouthEastResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerSouthEastResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private async Task DragEventHandlerSouthEastResizeHandle(MouseEventArgs mouseEventArgs)
    {
        var heightPixelOffset = Dimensions.HeightCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (heightPixelOffset is null)
        {
            heightPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.HeightCalc.Add(heightPixelOffset);
        }

        var deltaY = mouseEventArgs.ClientY - _previousDragMouseEventArgs!.ClientY;

        heightPixelOffset.Value += deltaY;
        
        var widthPixelOffset = Dimensions.WidthCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (widthPixelOffset is null)
        {
            widthPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.WidthCalc.Add(widthPixelOffset);
        }

        var deltaX = mouseEventArgs.ClientX - _previousDragMouseEventArgs!.ClientX;

        widthPixelOffset.Value += deltaX;
    }

    private void SubscribeToDragEventWithSouthWestResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerSouthWestResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private async Task DragEventHandlerSouthWestResizeHandle(MouseEventArgs mouseEventArgs)
    {
        var heightPixelOffset = Dimensions.HeightCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (heightPixelOffset is null)
        {
            heightPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.HeightCalc.Add(heightPixelOffset);
        }

        var deltaY = mouseEventArgs.ClientY - _previousDragMouseEventArgs!.ClientY;

        heightPixelOffset.Value += deltaY;

        var widthPixelOffset = Dimensions.WidthCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (widthPixelOffset is null)
        {
            widthPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.WidthCalc.Add(widthPixelOffset);
        }

        var deltaX = mouseEventArgs.ClientX - _previousDragMouseEventArgs!.ClientX;

        widthPixelOffset.Value -= deltaX;
        
        var leftPixelOffset = Dimensions.LeftCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (leftPixelOffset is null)
        {
            leftPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.LeftCalc.Add(leftPixelOffset);
        }

        leftPixelOffset.Value += deltaX;
    }

    private void SubscribeToDragEventWithNorthWestResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerNorthWestResizeHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private async Task DragEventHandlerNorthWestResizeHandle(MouseEventArgs mouseEventArgs)
    {
        var heightPixelOffset = Dimensions.HeightCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (heightPixelOffset is null)
        {
            heightPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.HeightCalc.Add(heightPixelOffset);
        }

        var deltaY = mouseEventArgs.ClientY - _previousDragMouseEventArgs!.ClientY;

        heightPixelOffset.Value -= deltaY;
        
        var topPixelOffset = Dimensions.TopCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (topPixelOffset is null)
        {
            topPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.TopCalc.Add(topPixelOffset);
        }

        topPixelOffset.Value += deltaY;

        var widthPixelOffset = Dimensions.WidthCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (widthPixelOffset is null)
        {
            widthPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.WidthCalc.Add(widthPixelOffset);
        }

        var deltaX = mouseEventArgs.ClientX - _previousDragMouseEventArgs!.ClientX;

        widthPixelOffset.Value -= deltaX;

        var leftPixelOffset = Dimensions.LeftCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (leftPixelOffset is null)
        {
            leftPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.LeftCalc.Add(leftPixelOffset);
        }

        leftPixelOffset.Value += deltaX;
    }

    public void SubscribeToDragEventWithMoveHandle()
    {
        _dragStateEventHandler = DragEventHandlerMoveHandle;
        Dispatcher.Dispatch(new SetDragStateAction(true, null));
    }

    private async Task DragEventHandlerMoveHandle(MouseEventArgs mouseEventArgs)
    {
        var deltaY = mouseEventArgs.ClientY - _previousDragMouseEventArgs!.ClientY;

        var topPixelOffset = Dimensions.TopCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (topPixelOffset is null)
        {
            topPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.TopCalc.Add(topPixelOffset);
        }

        topPixelOffset.Value += deltaY;

        var deltaX = mouseEventArgs.ClientX - _previousDragMouseEventArgs!.ClientX;

        var leftPixelOffset = Dimensions.LeftCalc.FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.Pixels);

        if (leftPixelOffset is null)
        {
            leftPixelOffset = new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Add,
                Value = 0
            };

            Dimensions.LeftCalc.Add(leftPixelOffset);
        }

        leftPixelOffset.Value += deltaX;
    }

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragState_StateChanged;
        TransformableOptionsStateWrap.StateChanged -= TransformableOptionsStateWrapOnStateChanged;

    }
}