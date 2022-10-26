using BlazorStudio.ClassLib.Store.DialogCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Settings;

public partial class SettingsDialogEntryPoint : ComponentBase
{
    private readonly DialogRecord _settingsDialog = new(
        DialogKey.NewDialogKey(),
        "Settings",
        typeof(SettingsDialog),
        null
    );

    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private void OpenSettingsDialogOnClick()
    {
        if (DialogStatesWrap.Value.List.All(x => x.DialogKey != _settingsDialog.DialogKey))
            Dispatcher.Dispatch(new RegisterDialogAction(_settingsDialog));
    }
}