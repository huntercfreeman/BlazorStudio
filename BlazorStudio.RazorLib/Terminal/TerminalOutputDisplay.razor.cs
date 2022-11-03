using System.Text;
using BlazorStudio.ClassLib.Html;
using BlazorStudio.ClassLib.State;
using BlazorStudio.ClassLib.Store.TerminalCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalOutputDisplay : FluxorComponent
{
    [Inject]
    private IStateSelection<TerminalSessionsState, TerminalSession?> TerminalSessionsStateSelection { get; set; } = null!;
    [Inject]
    private IState<TerminalSessionWasModifiedState> TerminalSessionWasModifiedStateWrap { get; set; } = null!;

    /// <summary>
    /// <see cref="TerminalSessionKey"/> is used to narrow down the terminal
    /// session.
    /// </summary>
    [Parameter, EditorRequired]
    public TerminalSessionKey TerminalSessionKey { get; set; } = null!;
    /// <summary>
    /// <see cref="TerminalCommandKey"/> is used to narrow down even further
    /// to the output of a specific command that was executed in a specific
    /// terminal session.
    /// <br/><br/>
    /// Optional
    /// </summary>
    [Parameter]
    public TerminalCommandKey? TerminalCommandKey { get; set; }

    protected override void OnInitialized()
    {
        TerminalSessionsStateSelection
            .Select(x =>
            {
                if (x.TerminalSessionMap
                    .TryGetValue(TerminalSessionKey, out var terminalSession))
                {
                    return terminalSession;
                }

                return null;
            });
        
        base.OnInitialized();
    }

    private MarkupString GetAllTextEscaped(string value)
    {
        return (MarkupString)value
            .Replace("\r\n", "\\r\\n<br/>")
            .Replace("\r", "\\r<br/>")
            .Replace("\n", "\\n<br/>")
            .Replace("\t", "--->")
            .Replace(" ", "·");
    }
    
    private static MarkupString ParseHttpLinks(string input)
    {
        var outputBuilder = new StringBuilder();

        var indexOfHttp = input.IndexOf("http");

        if (indexOfHttp > 0)
        {
            var firstSubstring = input.Substring(0, indexOfHttp);

            var httpBuilder = new StringBuilder();

            var position = indexOfHttp;

            while (position < input.Length)
            {
                var currentCharacter = input[position++];

                if (currentCharacter == ' ') break;

                httpBuilder.Append(currentCharacter);
            }

            var aTag = $"<a href=\"{httpBuilder}\" target=\"_blank\">{httpBuilder}</a>";

            var result = firstSubstring.EscapeHtml()
                         + aTag;

            if (position != input.Length - 1) result += input.Substring(position);

            outputBuilder.Append(result + "<br />");
        }
        else
            outputBuilder.Append(input.EscapeHtml() + "<br />");

        return (MarkupString)(outputBuilder.ToString());
    }
}