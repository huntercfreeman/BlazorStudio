using BlazorALaCarte.DialogNotification;
using BlazorALaCarte.DialogNotification.Store;
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
        null);

    public void DispatchRegisterDialogRecordAction()
    {
        Dispatcher.Dispatch(new DialogsState.RegisterDialogRecordAction(_dialogRecord));
    }
}