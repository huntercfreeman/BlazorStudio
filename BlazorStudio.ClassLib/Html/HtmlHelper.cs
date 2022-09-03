using System.Text;

namespace BlazorStudio.ClassLib.Html;

public static class HtmlHelper
{
	private static string _spaceString = "&nbsp;";
	private static string _tabString = "&nbsp;&nbsp;&nbsp;&nbsp;";
	private static string _newLineString = "<br/>";
	private static string _ampersandString = "&amp;";
	private static string _leftAngleBracketString = "&lt;";
	private static string _rightAngleBracketString = "&gt;";
	private static string _doubleQuoteString = "&quot;";
	private static string _singleQuoteString = "&#39;";

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
		return input.ToString()
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