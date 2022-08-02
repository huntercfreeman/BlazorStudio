using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorHeader : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter]
    public IPlainTextEditor PlainTextEditor { get; set; } = null!;

    private void FontSizeOnChanged(ChangeEventArgs changeEventArgs)
    {
        int fontSize = int.Parse(changeEventArgs.Value?.ToString() ?? string.Empty);

        Dispatcher.Dispatch(new PlainTextEditorSetFontSizeAction(PlainTextEditor.PlainTextEditorKey, fontSize));
    }
}