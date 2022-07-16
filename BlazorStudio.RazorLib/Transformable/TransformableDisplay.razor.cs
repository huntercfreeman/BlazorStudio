using System.Text;
using BlazorStudio.ClassLib.UserInterface;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Transformable;

public partial class TransformableDisplay : FluxorComponent
{
    [Inject]
    private IState<DragState> DragState { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public Dimensions Dimensions { get; set; } = null!;

    private static readonly DimensionUnit DEFAULT_HANDLE_SIZE_IN_PIXELS = new()
    {
        DimensionUnitKind = DimensionUnitKind.Pixels,
        Value = 7
    };
    private Action? _dragStateEventHandler;
    private Guid _transformativeDisplayId = Guid.NewGuid();

    private int _resizeEventCounter;

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
        DragState.StateChanged += DragState_StateChanged;

        base.OnInitialized();
    }

    private void DragState_StateChanged(object? sender, EventArgs e)
    {
        if (DragState.Value.MouseEventArgs is null)
        {
            _dragStateEventHandler = null;
        }
        else
        {
            if (_dragStateEventHandler is not null)
            {
                _dragStateEventHandler();
            }
        }
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
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };
        
        List<DimensionUnit> heightDimension = new()
        {
            new() 
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };

        List<DimensionUnit> leftDimension = new()
        {
            new() 
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
            }
        };
        
        List<DimensionUnit> topDimension = new()
        {
            new() 
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
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
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };
        
        List<DimensionUnit> heightDimension = new(Dimensions.HeightCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };
        
        List<DimensionUnit> leftDimension = new(Dimensions.WidthCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
            }
        };
        
        List<DimensionUnit> topDimension = new(Dimensions.WidthCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
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
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };

        List<DimensionUnit> heightDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };

        List<DimensionUnit> leftDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
            }
        };

        List<DimensionUnit> topDimension = new(Dimensions.HeightCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
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
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };

        List<DimensionUnit> heightDimension = new(Dimensions.HeightCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };

        List<DimensionUnit> leftDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
            }
        };

        List<DimensionUnit> topDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
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
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };
        
        List<DimensionUnit> heightDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };
        
        List<DimensionUnit> leftDimension = new(Dimensions.WidthCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
            }
        };
        
        List<DimensionUnit> topDimension = new(Dimensions.WidthCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
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
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };
        
        List<DimensionUnit> heightDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };
        
        List<DimensionUnit> leftDimension = new(Dimensions.WidthCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
            }
        };
        
        List<DimensionUnit> topDimension = new(Dimensions.HeightCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
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
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };
        
        List<DimensionUnit> heightDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };
        
        List<DimensionUnit> leftDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
            }
        };
        
        List<DimensionUnit> topDimension = new(Dimensions.HeightCalc)
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionUnitOperationKind = DimensionUnitOperationKind.Subtract,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
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
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };
        
        List<DimensionUnit> heightDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = DEFAULT_HANDLE_SIZE_IN_PIXELS.Value
            }
        };
        
        List<DimensionUnit> leftDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
            }
        };
        
        List<DimensionUnit> topDimension = new()
        {
            new()
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = -1 * DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0
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

    private void DispatchSubscribeToDragEventProviderStateAction(Action dragEventActionHandler)
    {
        var action = new SubscribeToDragEventProviderStateAction(_transformativeDisplayId,
            dragEventActionHandler);

        Dispatcher.Dispatch(action);
    }

    private void SubscribeToDragEventWithNorthResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerNorthResizeHandle;
        DispatchSubscribeToDragEventProviderStateAction(DragEventHandlerNorthResizeHandle);
    }

    private void DragEventHandlerNorthResizeHandle()
    {
        _resizeEventCounter++;

        var nextDimensionsRecord = HtmlElementRecord.DimensionsRecord with
        {
            Height =
                new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Height.Value - DragState.Value.DeltaY,
                    DimensionUnitKind.Pixels),
            Top =
                new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Top.Value + DragState.Value.DeltaY,
                    DimensionUnitKind.Pixels),
        };

        var replaceHtmlElementDimensionsRecordAction =
            new ReplaceHtmlElementDimensionsRecordAction(HtmlElementRecord.HtmlElementRecordKey, nextDimensionsRecord);

        Dispatcher.Dispatch(replaceHtmlElementDimensionsRecordAction);
    }

    private void SubscribeToDragEventWithEastResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerEastResizeHandle;
        DispatchSubscribeToDragEventProviderStateAction(DragEventHandlerEastResizeHandle);
    }

    private void DragEventHandlerEastResizeHandle()
    {
        _resizeEventCounter++;

        var nextDimensionsRecord = HtmlElementRecord.DimensionsRecord with
        {
            Width = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Width.Value + DragState.Value.DeltaX,
                DimensionUnitKind.Pixels)
        };

        var replaceHtmlElementDimensionsRecordAction =
            new ReplaceHtmlElementDimensionsRecordAction(HtmlElementRecord.HtmlElementRecordKey, nextDimensionsRecord);

        Dispatcher.Dispatch(replaceHtmlElementDimensionsRecordAction);
    }

    private void SubscribeToDragEventWithSouthResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerSouthResizeHandle;
        DispatchSubscribeToDragEventProviderStateAction(DragEventHandlerSouthResizeHandle);
    }

    private void DragEventHandlerSouthResizeHandle()
    {
        _resizeEventCounter++;

        var nextDimensionsRecord = HtmlElementRecord.DimensionsRecord with
        {
            Height = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Height.Value + DragState.Value.DeltaY,
                DimensionUnitKind.Pixels)
        };

        var replaceHtmlElementDimensionsRecordAction =
            new ReplaceHtmlElementDimensionsRecordAction(HtmlElementRecord.HtmlElementRecordKey, nextDimensionsRecord);

        Dispatcher.Dispatch(replaceHtmlElementDimensionsRecordAction);
    }

    private void SubscribeToDragEventWithWestResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerWestResizeHandle;
        DispatchSubscribeToDragEventProviderStateAction(DragEventHandlerWestResizeHandle);
    }

    private void DragEventHandlerWestResizeHandle()
    {
        _resizeEventCounter++;

        var nextDimensionsRecord = HtmlElementRecord.DimensionsRecord with
        {
            Width = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Width.Value - DragState.Value.DeltaX,
                DimensionUnitKind.Pixels),
            Left = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Left.Value + DragState.Value.DeltaX, DimensionUnitKind.Pixels)
        };

        var replaceHtmlElementDimensionsRecordAction =
            new ReplaceHtmlElementDimensionsRecordAction(HtmlElementRecord.HtmlElementRecordKey, nextDimensionsRecord);

        Dispatcher.Dispatch(replaceHtmlElementDimensionsRecordAction);
    }

    private void SubscribeToDragEventWithNorthEastResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerNorthEastResizeHandle;
        DispatchSubscribeToDragEventProviderStateAction(DragEventHandlerNorthEastResizeHandle);
    }

    private void DragEventHandlerNorthEastResizeHandle()
    {
        _resizeEventCounter++;

        var nextDimensionsRecord = HtmlElementRecord.DimensionsRecord with
        {
            Height = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Height.Value - DragState.Value.DeltaY,
                    DimensionUnitKind.Pixels),
            Top = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Top.Value + DragState.Value.DeltaY,
                    DimensionUnitKind.Pixels),
            Width = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Width.Value + DragState.Value.DeltaX,
                DimensionUnitKind.Pixels)
        };

        var replaceHtmlElementDimensionsRecordAction =
            new ReplaceHtmlElementDimensionsRecordAction(HtmlElementRecord.HtmlElementRecordKey, nextDimensionsRecord);

        Dispatcher.Dispatch(replaceHtmlElementDimensionsRecordAction);
    }

    private void SubscribeToDragEventWithSouthEastResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerSouthEastResizeHandle;
        DispatchSubscribeToDragEventProviderStateAction(DragEventHandlerSouthEastResizeHandle);
    }

    private void DragEventHandlerSouthEastResizeHandle()
    {
        _resizeEventCounter++;

        var nextDimensionsRecord = HtmlElementRecord.DimensionsRecord with
        {
            Height = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Height.Value + DragState.Value.DeltaY,
                DimensionUnitKind.Pixels),
            Width = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Width.Value + DragState.Value.DeltaX,
                DimensionUnitKind.Pixels)
        };

        var replaceHtmlElementDimensionsRecordAction =
            new ReplaceHtmlElementDimensionsRecordAction(HtmlElementRecord.HtmlElementRecordKey, nextDimensionsRecord);

        Dispatcher.Dispatch(replaceHtmlElementDimensionsRecordAction);
    }

    private void SubscribeToDragEventWithSouthWestResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerSouthWestResizeHandle;
        DispatchSubscribeToDragEventProviderStateAction(DragEventHandlerSouthWestResizeHandle);
    }

    private void DragEventHandlerSouthWestResizeHandle()
    {
        _resizeEventCounter++;

        var nextDimensionsRecord = HtmlElementRecord.DimensionsRecord with
        {
            Height = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Height.Value + DragState.Value.DeltaY,
                DimensionUnitKind.Pixels),
            Width = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Width.Value - DragState.Value.DeltaX,
                DimensionUnitKind.Pixels),
            Left = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Left.Value + DragState.Value.DeltaX, DimensionUnitKind.Pixels)
        };

        var replaceHtmlElementDimensionsRecordAction =
            new ReplaceHtmlElementDimensionsRecordAction(HtmlElementRecord.HtmlElementRecordKey, nextDimensionsRecord);

        Dispatcher.Dispatch(replaceHtmlElementDimensionsRecordAction);
    }

    private void SubscribeToDragEventWithNorthWestResizeHandle()
    {
        _dragStateEventHandler = DragEventHandlerNorthWestResizeHandle;
        DispatchSubscribeToDragEventProviderStateAction(DragEventHandlerNorthWestResizeHandle);
    }

    private void DragEventHandlerNorthWestResizeHandle()
    {
        _resizeEventCounter++;

        var nextDimensionsRecord = HtmlElementRecord.DimensionsRecord with
        {
            Height = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Height.Value - DragState.Value.DeltaY,
                    DimensionUnitKind.Pixels),
            Top = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Top.Value + DragState.Value.DeltaY,
                    DimensionUnitKind.Pixels),
            Width = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Width.Value - DragState.Value.DeltaX,
                DimensionUnitKind.Pixels),
            Left = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Left.Value + DragState.Value.DeltaX, DimensionUnitKind.Pixels)
        };

        var replaceHtmlElementDimensionsRecordAction =
            new ReplaceHtmlElementDimensionsRecordAction(HtmlElementRecord.HtmlElementRecordKey, nextDimensionsRecord);

        Dispatcher.Dispatch(replaceHtmlElementDimensionsRecordAction);
    }

    public void SubscribeToDragEventWithMoveHandle()
    {
        _dragStateEventHandler = DragEventHandlerMoveHandle;
        DispatchSubscribeToDragEventProviderStateAction(DragEventHandlerMoveHandle);
    }

    private void DragEventHandlerMoveHandle()
    {
        _resizeEventCounter++;

        var nextDimensionsRecord = HtmlElementRecord.DimensionsRecord with
        {
            Top = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Top.Value + DragState.Value.DeltaY,
                    DimensionUnitKind.Pixels),
            Left = new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Left.Value + DragState.Value.DeltaX,
                    DimensionUnitKind.Pixels)
        };

        var replaceHtmlElementDimensionsRecordAction =
            new ReplaceHtmlElementDimensionsRecordAction(HtmlElementRecord.HtmlElementRecordKey, nextDimensionsRecord);

        Dispatcher.Dispatch(replaceHtmlElementDimensionsRecordAction);
    }

    private void ValidateDimensionUnitKindIsSupported(string dimensionName,
        DimensionValuedUnit dimensionValuedUnit)
    {
        if (dimensionValuedUnit.DimensionUnitKind != DimensionUnitKind.Pixels)
            throw new ApplicationException($"The {nameof(DimensionUnitKind)}: {dimensionValuedUnit.DimensionUnitKind} " +
                $"is not supported for {nameof(TransformativeDisplay)}. The name of the dimension with this " +
                $"unsupported type is named: {dimensionName}.");
    }

    protected override void Dispose(bool disposing)
    {
        DragState.StateChanged -= DragState_StateChanged;

        base.Dispose(disposing);
    }
}