using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.RazorLib.ResizableCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.DialogCase;

public partial class DialogDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private ResizableDisplay? _resizableDisplay;

    private string StyleCssString => DialogRecord.ElementDimensions.StyleString;
    
    private async Task ReRenderAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    private void SubscribeMoveHandle()
    {
        _resizableDisplay?.SubscribeToDragEventWithMoveHandle();
    }
    
    private void DispatchDisposeDialogRecordAction()
    {
        Dispatcher.Dispatch(new DisposeDialogRecordAction(DialogRecord));
    }
}