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
    [Parameter]
    public PlainTextEditorDisplay.WidthAndHeightTestResult? WidthAndHeightTestResult { get; set; }

    private DialogRecord _diffDialog = null!;

    protected override void OnParametersSet()
    {
        if (_diffDialog is null)
        {
            _diffDialog = new DialogRecord(
                DialogKey.NewDialogKey(),
                "Diff",
                typeof(DiffDialog),
                new Dictionary<string, object?>
                {
                    {
                        nameof(DiffDialog.PlainTextEditorKey),
                        PlainTextEditorKey
                    },
                    {
                        nameof(DiffDialog.WidthAndHeightTestResult),
                        WidthAndHeightTestResult
                    }
                }
            );
        }
        else
        {
            _diffDialog = _diffDialog with
            {
                Parameters = new Dictionary<string, object?>
                {
                    {
                        nameof(DiffDialog.PlainTextEditorKey),
                        PlainTextEditorKey
                    },
                    {
                        nameof(DiffDialog.WidthAndHeightTestResult),
                        WidthAndHeightTestResult
                    }
                }
            };
        }

        base.OnParametersSet();
    }

    private void OpenDiffDialogOnClick()
    {
        if (DialogStatesWrap.Value.List.All(x => x.DialogKey != _diffDialog.DialogKey))
            Dispatcher.Dispatch(new RegisterDialogAction(_diffDialog));
    }
}