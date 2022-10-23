using System.Collections.Immutable;
using Blazor.Text.Editor.Analysis.Shared;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;

public static class HtmlFacts
{
    public const char SPECIAL_HTML_TAG = '!';
    
    public const char OPEN_TAG_BEGINNING = '<';
    
    public const string OPEN_TAG_WITH_CHILD_CONTENT_ENDING = ">";
    public const string OPEN_TAG_SELF_CLOSING_ENDING = "/>";
    
    public const string CLOSE_TAG_WITH_CHILD_CONTENT_BEGINNING = "</";
    
    public static readonly ImmutableArray<string> OPEN_TAG_ENDING_OPTIONS = new []
    {
        OPEN_TAG_WITH_CHILD_CONTENT_ENDING,
        OPEN_TAG_SELF_CLOSING_ENDING
    }.ToImmutableArray();
    
    public static readonly ImmutableArray<string> WHITESPACE = new []
    {
        " ",
        "\t",
        "\r",
        "\n"
    }.ToImmutableArray();
    
    public static readonly ImmutableArray<string> TAG_NAME_STOP_DELIMITERS = new []
        {
            ParserFacts.END_OF_FILE.ToString()
        }
        .Union(WHITESPACE)
        .Union(OPEN_TAG_ENDING_OPTIONS)
        .ToImmutableArray();
}