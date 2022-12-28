using BlazorALaCarte.DialogNotification;
using BlazorALaCarte.DialogNotification.Dialog;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Settings;

public partial class SettingsDialogEntryPoint : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private DialogRecord _dialogRecord = new(
        DialogKey.NewDialogKey(), 
        "Settings",
        typeof(SettingsDisplay),
        null)
    {
        IsResizable = true
    };

    public void DispatchRegisterDialogRecordAction()
    {
        Dispatcher.Dispatch(new DialogsState.RegisterDialogRecordAction(_dialogRecord));
    }
}