using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.RazorLib.Transformable;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Dialog;

public partial class DialogDisplay : ComponentBase
{
    [Parameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private TransformableDisplay _transformableDisplay = null!;

    private void FireSubscribeToDragEventWithMoveHandle()
    {
        _transformableDisplay.SubscribeToDragEventWithMoveHandle();
    }
}