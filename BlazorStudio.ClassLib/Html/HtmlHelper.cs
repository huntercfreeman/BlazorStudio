using System.Text;

namespace BlazorStudio.ClassLib.Html;

public static class HtmlHelper
{
    private static readonly string _spaceString = "&nbsp;";
    private static readonly string _tabString = "&nbsp;&nbsp;&nbsp;&nbsp;";
    private static readonly string _newLineString = "<br/>";
    private static readonly string _ampersandString = "&amp;";
    private static readonly string _leftAngleBracketString = "&lt;";
    private static readonly string _rightAngleBracketString = "&gt;";
    private static readonly string _doubleQuoteString = "&quot;";
    private static readonly string _singleQuoteString = "&#39;";

    public static string EscapeHtml(this char input)
    {
        return input.ToString().EscapeHtml();
    }

    public static string EscapeHtml(this StringBuilder input)
    {
        return input.ToString().EscapeHtml();
    }

    public static string EscapeHtml(this string input)
    {
        return input
            .Replace("&", _ampersandString)
            .Replace("<", _leftAngleBracketString)
            .Replace(">", _rightAngleBracketString)
            .Replace("\t", _tabString)
            .Replace(" ", _spaceString)
            .Replace("\r\n", _newLineString)
            .Replace("\n", _newLineString)
            .Replace("\r", _newLineString)
            .Replace("\"", _doubleQuoteString)
            .Replace("'", _singleQuoteString);
    }
}