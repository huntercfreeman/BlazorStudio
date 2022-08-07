using System.Text;
using BlazorStudio.ClassLib.Html;

namespace BlazorStudio.ClassLib.Store.TerminalCase;

public static class TerminalOutputParser
{
    public static string ParseHttpLinks(string input)
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

                if (currentCharacter == ' ')
                {
                    break;
                }
                else
                {
                    httpBuilder.Append(currentCharacter);
                }
            }

            var aTag = $"<a href=\"{httpBuilder}\">{httpBuilder}</a>";

            var result = firstSubstring.EscapeHtml()
                         + aTag.ToString();

            if (position != input.Length - 1)
            {
                result += input.Substring(position);
            }

            outputBuilder.Append(result + "<br />");
        }
        else
        {
            outputBuilder.Append(input.EscapeHtml() + "<br />");
        }

        return outputBuilder.ToString();
    }
}