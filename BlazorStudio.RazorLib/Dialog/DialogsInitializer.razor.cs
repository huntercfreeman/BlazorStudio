using BlazorStudio.ClassLib.Contexts;
using BlazorStudio.ClassLib.Store.ContextCase;
using BlazorStudio.ClassLib.Store.DialogCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Dialog;

public partial class DialogsInitializer : FluxorComponent
{
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    protected override void Dispose(bool disposing)
    {
        var dialogStates = DialogStatesWrap.Value;

        foreach (var dialog in dialogStates.List)
        {
            Dispatcher.Dispatch(new DisposeDialogAction(dialog));
        }

        base.Dispose(disposing);
    }
}