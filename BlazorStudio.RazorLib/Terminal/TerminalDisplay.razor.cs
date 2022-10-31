using BlazorStudio.ClassLib.Store.TerminalCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalDisplay : FluxorComponent
{
    [Inject]
    private IState<TerminalResultState> TerminalResultStateWrap { get; set; } = null!;
    
    private MarkupString GetAllTextEscaped(string value)
    {
        return (MarkupString)value
            .Replace("\r\n", "\\r\\n<br/>")
            .Replace("\r", "\\r<br/>")
            .Replace("\n", "\\n<br/>")
            .Replace("\t", "--->")
            .Replace(" ", "·");
    }
}