using BlazorStudio.ClassLib.Store.DialogCase;
using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.RazorLib.Settings;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class DiffDialogEntryPoint : ComponentBase
{
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public PlainTextEditorKey PlainTextEditorKey { get; set; } = null!;

    private readonly DialogRecord _diffDialog = new DialogRecord(
        DialogKey.NewDialogKey(),
        "Diff",
        typeof(DiffDialog),
        null
    );

    private void OpenDiffDialogOnClick()
    {
        if (DialogStatesWrap.Value.List.All(x => x.DialogKey != _diffDialog.DialogKey))
            Dispatcher.Dispatch(new RegisterDialogAction(_diffDialog));
    }
}