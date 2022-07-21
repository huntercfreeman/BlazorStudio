using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.RazorLib.Settings;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Shared;

public partial class DebuggingVirtualizeCoordinateSystemEntryPoint : ComponentBase
{
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private readonly DialogRecord _settingsDialog = new DialogRecord(
        DialogKey.NewDialogKey(),
        "Debugging VirtualizeCoordinateSystem<T>",
        typeof(DebuggingVirtualizeCoordinateSystemWrapper),
        null
    );

    private void OpenSettingsDialogOnClick()
    {
        if (DialogStatesWrap.Value.List.All(x => x.DialogKey != _settingsDialog.DialogKey))
            Dispatcher.Dispatch(new RegisterDialogAction(_settingsDialog));
    }
}