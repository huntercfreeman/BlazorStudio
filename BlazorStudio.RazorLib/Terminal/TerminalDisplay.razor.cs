using System.Text;
using BlazorStudio.ClassLib.Html;
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