using System.Collections.Immutable;
using Blazor.Text.Editor.Analysis.Shared;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;

public static class HtmlFacts
{
    public const char TAG_OPENING_CHARACTER = '<';
    public const char SPECIAL_HTML_TAG_CHARACTER = '!';
    
    public const string TAG_CLOSING_OPTION_ONE = ">";
    public const string TAG_CLOSING_OPTION_TWO = "/>";
    
    public static readonly ImmutableArray<string> TAG_CLOSING_OPTIONS = new []
    {
        TAG_CLOSING_OPTION_ONE,
        TAG_CLOSING_OPTION_TWO
    }.ToImmutableArray();
    
    public static readonly ImmutableArray<string> HTML_WHITESPACE = new []
    {
        " ",
        "\t",
        "\r",
        "\n"
    }.ToImmutableArray();
    
    public static readonly ImmutableArray<string> END_OF_TAG_NAME_DELIMITERS = new string[]
        {
            ParserFacts.END_OF_FILE.ToString()
        }
        .Union(TAG_CLOSING_OPTIONS)
        .ToImmutableArray();
}