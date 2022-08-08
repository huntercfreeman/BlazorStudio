using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using Microsoft.AspNetCore.Components;
using static BlazorStudio.RazorLib.PlainTextEditorCase.PlainTextEditorDisplay;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorDebuggingInformation : ComponentBase
{
    [Parameter]
    public IPlainTextEditor PlainTextEditor { get; set; } = null!;
    [Parameter]
    public WidthAndHeightTestResult? WidthAndHeightTestResult { get; set; }

    private MarkupString NullSafeToMarkupString(string name, object? obj)
    {
        return (MarkupString)(name + "&nbsp;->&nbsp;" + obj?.ToString() ?? "null");
    }
}