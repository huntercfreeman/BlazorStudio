using System.Collections.Immutable;
using Blazor.Text.Editor.Analysis.Shared;

namespace Blazor.Text.Editor.Analysis.Html.ClassLib.Syntax;

public static class HtmlFacts
{
    public const char START_OPEN_TAG = '<';
    public const char SPECIAL_HTML_TAG = '!';
    
    public const string END_OPEN_TAG_WITH_CHILD_CONTENT = ">";
    public const string END_OPEN_TAG_SELF_CLOSING = "/>";
    
    public const string START_CLOSE_TAG_WITH_CHILD_CONTENT = "</";
    
    public static readonly ImmutableArray<string> END_OPEN_TAG_OPTIONS = new []
    {
        END_OPEN_TAG_WITH_CHILD_CONTENT,
        END_OPEN_TAG_SELF_CLOSING
    }.ToImmutableArray();
    
    public static readonly ImmutableArray<string> HTML_WHITESPACE = new []
    {
        " ",
        "\t",
        "\r",
        "\n"
    }.ToImmutableArray();
    
    public static readonly ImmutableArray<string> END_TAG_NAME_DELIMITERS = new string[]
        {
            ParserFacts.END_OF_FILE.ToString()
        }
        .Union(END_OPEN_TAG_OPTIONS)
        .ToImmutableArray();
}