using BlazorStudio.ClassLib.Store.PlainTextEditorCase;
using BlazorStudio.ClassLib.Store.SolutionCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using static BlazorStudio.RazorLib.PlainTextEditorCase.PlainTextEditorDisplay;

namespace BlazorStudio.RazorLib.PlainTextEditorCase;

public partial class PlainTextEditorDebuggingInformation : FluxorComponent
{
    [Inject] 
    private IState<SolutionState> SolutionStateWrap { get; set; } = null!;
    
    [Parameter]
    public IPlainTextEditor PlainTextEditor { get; set; } = null!;
    [Parameter]
    public WidthAndHeightTestResult? WidthAndHeightTestResult { get; set; }

    private MarkupString NullSafeToMarkupString(string name, object? obj)
    {
        return (MarkupString)(name + "&nbsp;->&nbsp;" + obj?.ToString() ?? "null");
    }
}