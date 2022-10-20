using System.Collections.Immutable;
using Blazor.Text.Editor.Analysis.Shared;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;

public static class HtmlFacts
{
    public const char TAG_OPENING_CHARACTER = '<';
    public const char SPECIAL_HTML_TAG_CHARACTER = '!';
    
    public const string OPEN_TAG_ENDING_CHILD_CONTENT = ">";
    public const string OPEN_TAG_ENDING_SELF_CLOSING = "/>";
    
    public static readonly ImmutableArray<string> TAG_CLOSING_OPTIONS = new []
    {
        OPEN_TAG_ENDING_CHILD_CONTENT,
        OPEN_TAG_ENDING_SELF_CLOSING
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