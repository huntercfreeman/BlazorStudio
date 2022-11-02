using System.Text;
using BlazorStudio.ClassLib.Html;
using BlazorStudio.ClassLib.Store.TerminalCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorStudio.RazorLib.Terminal;

public partial class TerminalOutputDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IStateSelection<TerminalResultState, TerminalCommand?> TerminalResultSelection { get; set; } = null!;

    [Parameter, EditorRequired]
    public TerminalCommandKey TerminalCommandKey { get; set; } = null!;
    
    protected override void OnInitialized()
    {
        TerminalResultSelection
            .Select(x =>
            {
                if (x.TerminalCommandResultMap
                    .TryGetValue(TerminalCommandKey, out var terminalCommand))
                {
                    return terminalCommand;
                }

                return null;
            });
        
        TerminalResultSelection.SelectedValueChanged += TerminalResultSelectionOnSelectedValueChanged;
        
        base.OnInitialized();
    }

    private void TerminalResultSelectionOnSelectedValueChanged(object? sender, TerminalCommand? e)
    {
        InvokeAsync(StateHasChanged);
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

    public void Dispose()
    {
        TerminalResultSelection.SelectedValueChanged -= TerminalResultSelectionOnSelectedValueChanged;
    }
}