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
        var cssStylingBuilder = new StringBuilder();

        List<DimensionUnit> widthInPixels = new(Dimensions.WidthCalc)
        {
            new() 
            {
                Value = 
            }
        };
        
        DimensionValuedUnit(
                HtmlElementRecord.DimensionsRecord.Width.Value - DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
                DimensionUnitKind.Pixels);}

        DimensionValuedUnit heightInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit leftInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit topInPixels =
            new DimensionValuedUnit(-1 * DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        cssStylingBuilder.Append($"width: {widthInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"height: {heightInPixels.BuildCssStyleString()}; ");

        cssStylingBuilder.Append($"left: {leftInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"top: {topInPixels.BuildCssStyleString()}; ");

        return cssStylingBuilder.ToString();
    }

    private string GetEastResizeHandleCssStyling()
    {
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Width), HtmlElementRecord.DimensionsRecord.Width);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Height), HtmlElementRecord.DimensionsRecord.Height);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Left), HtmlElementRecord.DimensionsRecord.Left);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Top), HtmlElementRecord.DimensionsRecord.Top);

        var cssStylingBuilder = new StringBuilder();

        DimensionValuedUnit widthInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
                DimensionUnitKind.Pixels);

        DimensionValuedUnit heightInPixels =
            new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Height.Value - DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit leftInPixels =
            new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Width.Value - DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit topInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        cssStylingBuilder.Append($"width: {widthInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"height: {heightInPixels.BuildCssStyleString()}; ");

        cssStylingBuilder.Append($"left: {leftInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"top: {topInPixels.BuildCssStyleString()}; ");

        return cssStylingBuilder.ToString();
    }

    private string GetSouthResizeHandleCssStyling()
    {
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Width), HtmlElementRecord.DimensionsRecord.Width);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Height), HtmlElementRecord.DimensionsRecord.Height);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Left), HtmlElementRecord.DimensionsRecord.Left);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Top), HtmlElementRecord.DimensionsRecord.Top);

        var cssStylingBuilder = new StringBuilder();

        DimensionValuedUnit widthInPixels =
            new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Width.Value - DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
                DimensionUnitKind.Pixels);

        DimensionValuedUnit heightInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit leftInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit topInPixels =
            new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Height.Value - DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        cssStylingBuilder.Append($"width: {widthInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"height: {heightInPixels.BuildCssStyleString()}; ");

        cssStylingBuilder.Append($"left: {leftInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"top: {topInPixels.BuildCssStyleString()}; ");

        return cssStylingBuilder.ToString();
    }

    private string GetWestResizeHandleCssStyling()
    {
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Width), HtmlElementRecord.DimensionsRecord.Width);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Height), HtmlElementRecord.DimensionsRecord.Height);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Left), HtmlElementRecord.DimensionsRecord.Left);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Top), HtmlElementRecord.DimensionsRecord.Top);

        var cssStylingBuilder = new StringBuilder();

        DimensionValuedUnit widthInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
                DimensionUnitKind.Pixels);

        DimensionValuedUnit heightInPixels =
            new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Height.Value - DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit leftInPixels =
            new DimensionValuedUnit(-1 * DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit topInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        cssStylingBuilder.Append($"width: {widthInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"height: {heightInPixels.BuildCssStyleString()}; ");

        cssStylingBuilder.Append($"left: {leftInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"top: {topInPixels.BuildCssStyleString()}; ");

        return cssStylingBuilder.ToString();
    }

    private string GetNorthEastResizeHandleCssStyling()
    {
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Width), HtmlElementRecord.DimensionsRecord.Width);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Height), HtmlElementRecord.DimensionsRecord.Height);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Left), HtmlElementRecord.DimensionsRecord.Left);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Top), HtmlElementRecord.DimensionsRecord.Top);

        var cssStylingBuilder = new StringBuilder();

        DimensionValuedUnit widthInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
                DimensionUnitKind.Pixels);

        DimensionValuedUnit heightInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit leftInPixels =
            new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Width.Value - DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit topInPixels =
            new DimensionValuedUnit(-1 * DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        cssStylingBuilder.Append($"width: {widthInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"height: {heightInPixels.BuildCssStyleString()}; ");

        cssStylingBuilder.Append($"left: {leftInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"top: {topInPixels.BuildCssStyleString()}; ");

        return cssStylingBuilder.ToString();
    }

    private string GetSouthEastResizeHandleCssStyling()
    {
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Width), HtmlElementRecord.DimensionsRecord.Width);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Height), HtmlElementRecord.DimensionsRecord.Height);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Left), HtmlElementRecord.DimensionsRecord.Left);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Top), HtmlElementRecord.DimensionsRecord.Top);

        var cssStylingBuilder = new StringBuilder();

        DimensionValuedUnit widthInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
                DimensionUnitKind.Pixels);

        DimensionValuedUnit heightInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit leftInPixels =
            new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Width.Value - DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit topInPixels =
            new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Height.Value - DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        cssStylingBuilder.Append($"width: {widthInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"height: {heightInPixels.BuildCssStyleString()}; ");

        cssStylingBuilder.Append($"left: {leftInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"top: {topInPixels.BuildCssStyleString()}; ");

        return cssStylingBuilder.ToString();
    }

    private string GetSouthWestResizeHandleCssStyling()
    {
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Width), HtmlElementRecord.DimensionsRecord.Width);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Height), HtmlElementRecord.DimensionsRecord.Height);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Left), HtmlElementRecord.DimensionsRecord.Left);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Top), HtmlElementRecord.DimensionsRecord.Top);

        var cssStylingBuilder = new StringBuilder();

        DimensionValuedUnit widthInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
                DimensionUnitKind.Pixels);

        DimensionValuedUnit heightInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit leftInPixels =
            new DimensionValuedUnit(-1 * DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit topInPixels =
            new DimensionValuedUnit(HtmlElementRecord.DimensionsRecord.Height.Value - DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        cssStylingBuilder.Append($"width: {widthInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"height: {heightInPixels.BuildCssStyleString()}; ");

        cssStylingBuilder.Append($"left: {leftInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"top: {topInPixels.BuildCssStyleString()}; ");

        return cssStylingBuilder.ToString();
    }

    private string GetNorthWestResizeHandleCssStyling()
    {
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Width), HtmlElementRecord.DimensionsRecord.Width);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Height), HtmlElementRecord.DimensionsRecord.Height);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Left), HtmlElementRecord.DimensionsRecord.Left);
        ValidateDimensionUnitKindIsSupported(nameof(DimensionsRecord.Top), HtmlElementRecord.DimensionsRecord.Top);

        var cssStylingBuilder = new StringBuilder();

        DimensionValuedUnit widthInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
                DimensionUnitKind.Pixels);

        DimensionValuedUnit heightInPixels =
            new DimensionValuedUnit(DEFAULT_HANDLE_SIZE_IN_PIXELS.Value,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit leftInPixels =
            new DimensionValuedUnit(-1 * DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        DimensionValuedUnit topInPixels =
            new DimensionValuedUnit(-1 * DEFAULT_HANDLE_SIZE_IN_PIXELS.Value / 2.0,
            DimensionUnitKind.Pixels);

        cssStylingBuilder.Append($"width: {widthInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"height: {heightInPixels.BuildCssStyleString()}; ");

        cssStylingBuilder.Append($"left: {leftInPixels.BuildCssStyleString()}; ");
        cssStylingBuilder.Append($"top: {topInPixels.BuildCssStyleString()}; ");

        return cssStylingBuilder.ToString();
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