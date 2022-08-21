using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.UserInterface;
using BlazorStudio.RazorLib.ContextCase;
using BlazorStudio.RazorLib.Transformable;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.Dialog;

public partial class DialogDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IStateSelection<ContextState, ContextRecord> ContextStateSelector { get; set; } = null!;
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private TransformableDisplay? _transformableDisplay;
    private ContextBoundary? _contextBoundary;

    private string CssStyleString => $"{OverrideZIndex} {DialogRecord.Dimensions.DimensionsCssString}";
    
    private string OverrideZIndex => DialogStatesWrap.Value.DialogKeyWithOverridenZIndex is not null &&
                                     DialogStatesWrap.Value.DialogKeyWithOverridenZIndex == DialogRecord.DialogKey
        ? "z-index: 11;"
        : string.Empty;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            DialogRecord.OnFocusRequestedEventHandler += DialogRecordOnOnFocusRequestedEventHandler;
            DialogRecord.OnStateHasChangeRequestedEventHandler += DialogRecordOnOnStateHasChangeRequestedEventHandler;
        }
        
        return base.OnAfterRenderAsync(firstRender);
    }

    private async void DialogRecordOnOnFocusRequestedEventHandler(object? sender, EventArgs e)
    {
        if (_contextBoundary is not null)
        {
            await _contextBoundary.HandleOnFocusInAsync(null);
            await InvokeAsync(StateHasChanged);
        }
    }
    
    private async void DialogRecordOnOnStateHasChangeRequestedEventHandler(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

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
    
    private Task HandleOnFocusInAsync(FocusEventArgs focusEventArgs)
    {
        _contextBoundary.DispatchSetActiveContextStatesAction(new());

        var dialogStates = DialogStatesWrap.Value;

        if (dialogStates.DialogKeyWithOverridenZIndex is not null &&
            dialogStates.DialogKeyWithOverridenZIndex != DialogRecord.DialogKey)
        {
            var dialog = dialogStates.List
                .FirstOrDefault(d => d.DialogKey == dialogStates.DialogKeyWithOverridenZIndex);

            dialogStates.DialogKeyWithOverridenZIndex = null;
            
            if (dialog is not null)
                dialog.InvokeOnStateHasChangeRequestedEventHandler();
        }
        
        DialogStatesWrap.Value.DialogKeyWithOverridenZIndex = DialogRecord.DialogKey;
        DialogStatesWrap.Value.MostRecentlyFocusedDialogKey = DialogRecord.DialogKey;

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        DialogRecord.OnFocusRequestedEventHandler -= DialogRecordOnOnFocusRequestedEventHandler;
        DialogRecord.OnStateHasChangeRequestedEventHandler -= DialogRecordOnOnStateHasChangeRequestedEventHandler;
    }
}