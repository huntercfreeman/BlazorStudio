using BlazorStudio.ClassLib.Store.DialogCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Dialog;

public partial class DialogTaskBarEntry : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private void ToggleMinimizeOnClick()
    {
        Dispatcher.Dispatch(new ReplaceDialogAction(DialogRecord,
            DialogRecord with
            {
                IsMinimized = !DialogRecord.IsMinimized,
            }));
    }

    private void CloseOnClick()
    {
        Dispatcher.Dispatch(new DisposeDialogAction(DialogRecord));
    }
}