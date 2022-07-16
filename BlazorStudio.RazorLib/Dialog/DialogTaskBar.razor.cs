using BlazorStudio.ClassLib.Store.DialogCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Dialog;

public partial class DialogTaskBar : FluxorComponent
{
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
}