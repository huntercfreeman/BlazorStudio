using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class TextTokenDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ITextToken TextToken { get; set; } = null!;
}
