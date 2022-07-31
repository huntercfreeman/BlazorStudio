using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorHeader : ComponentBase
{
    [Parameter]
    public IPlainTextEditor PlainTextEditor { get; set; } = null!;
}