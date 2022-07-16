using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.RazorLib.Transformable;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Dialog;

public partial class DialogDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private TransformableDisplay? _transformableDisplay;

    private void FireSubscribeToDragEventWithMoveHandle()
    {
        if (_transformableDisplay is not null)
        {
            _transformableDisplay.SubscribeToDragEventWithMoveHandle();
        }
    }

    private async Task ReRender()
    {
        await InvokeAsync(StateHasChanged);
    }
    
    private void MinimizeOnClick()
    {
        Dispatcher.Dispatch(new ReplaceDialogAction(DialogRecord,
            DialogRecord with
            {
                IsMinimized = true
            }));
    }
    
    private void MaximizeOnClick()
    {

        double widthHeightViewportAmount = DialogRecord.IsMaximized ? 60 : 100;
        double leftTopViewportAmount = DialogRecord.IsMaximized ? 20 : 0;
        bool isDisabled = !DialogRecord.IsMaximized;
        DialogRecord.IsTransformable = DialogRecord.IsMaximized;

        DialogRecord.IsMaximized = !DialogRecord.IsMaximized;


        foreach (var widthDimension in DialogRecord.Dimensions.WidthCalc)
        {
            widthDimension.IsDisabled = isDisabled;
        }
        
        foreach (var heightDimension in DialogRecord.Dimensions.HeightCalc)
        {
            heightDimension.IsDisabled = isDisabled;
        }
        
        foreach (var leftDimension in DialogRecord.Dimensions.LeftCalc)
        {
            leftDimension.IsDisabled = isDisabled;
        }
        
        foreach (var topDimension in DialogRecord.Dimensions.TopCalc)
        {
            topDimension.IsDisabled = isDisabled;
        }

        var viewportWidthDimension = DialogRecord.Dimensions.WidthCalc
            .FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.ViewportWidth);

        if (viewportWidthDimension is not null)
        {
            viewportWidthDimension.IsDisabled = false;
            viewportWidthDimension.Value = widthHeightViewportAmount;
        }
        
        var viewportHeightDimension = DialogRecord.Dimensions.HeightCalc
            .FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.ViewportHeight);

        if (viewportHeightDimension is not null)
        {
            viewportHeightDimension.IsDisabled = false;
            viewportHeightDimension.Value = widthHeightViewportAmount;
        }
        
        var viewportLeftDimension = DialogRecord.Dimensions.LeftCalc
            .FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.ViewportWidth);

        if (viewportLeftDimension is not null)
        {
            viewportLeftDimension.IsDisabled = false;
            viewportLeftDimension.Value = leftTopViewportAmount;
        }
        
        var viewportTopDimension = DialogRecord.Dimensions.TopCalc
            .FirstOrDefault(x => x.DimensionUnitKind == DimensionUnitKind.ViewportHeight);

        if (viewportTopDimension is not null)
        {
            viewportTopDimension.IsDisabled = false;
            viewportTopDimension.Value = leftTopViewportAmount;
        }
    }
    
    private void CloseOnClick()
    {
        Dispatcher.Dispatch(new DisposeDialogAction(DialogRecord));
    }
}