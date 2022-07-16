using BlazorStudio.ClassLib.Store.DialogCase;
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

    private TransformableDisplay _transformableDisplay = null!;

    private void FireSubscribeToDragEventWithMoveHandle()
    {
        _transformableDisplay.SubscribeToDragEventWithMoveHandle();
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
}