using BlazorStudio.ClassLib.Store.DialogCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Settings;

public partial class SettingsDialogEntryPoint : ComponentBase
{
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private DialogRecord _settingsDialog = new DialogRecord(
        DialogKey.NewDialogKey(),
        "Settings",
        typeof(SettingsDialog),
        null
    );

    private void OpenSettingsDialogOnClick()
    {
        if (!DialogStatesWrap.Value.List.Contains(_settingsDialog))
            Dispatcher.Dispatch(new RegisterDialogAction(_settingsDialog));
    }
}