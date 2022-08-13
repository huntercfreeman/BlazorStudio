using BlazorStudio.ClassLib.FileSystem.Interfaces;
using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.RazorLib.Settings;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStudio.RazorLib.InputFile;

public partial class InputFileDialogEntryPoint : ComponentBase
{
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private DialogRecord _inputFileDialog = new DialogRecord(
        DialogKey.NewDialogKey(),
        "Input File",
        typeof(InputFileDialog),
        null
    );

    private void OpenInputFileDialogOnClick()
    {
        if (DialogStatesWrap.Value.List.All(x => x.DialogKey != _inputFileDialog.DialogKey))
            Dispatcher.Dispatch(new RegisterDialogAction(_inputFileDialog));
    }
}