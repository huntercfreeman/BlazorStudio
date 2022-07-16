using BlazorStudio.ClassLib.Store.DialogCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Settings;

public partial class SettingsDialogEntryPoint : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private void OpenSettingsDialogOnClick()
    {
        Dispatcher.Dispatch(new RegisterDialogAction(new DialogRecord(
            DialogKey.NewDialogKey(), 
            "Settings",
            typeof(SettingsDialog),
            null
        )));
    }
}