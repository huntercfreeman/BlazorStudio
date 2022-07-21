using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class CountOfPlainTextEditors : FluxorComponent
{
    [Inject]
    private IState<PlainTextEditorStates> PlainTextEditorStatesWrap { get; set; } = null!;
}
