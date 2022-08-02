using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorDebuggingInformation : ComponentBase
{
    [Parameter]
    public IPlainTextEditor PlainTextEditor { get; set; } = null!;

    private MarkupString NullSafeToMarkupString(string name, object? obj)
    {
        return (MarkupString)(name + "&nbsp;->&nbsp;" + obj?.ToString() ?? "null");
    }
}